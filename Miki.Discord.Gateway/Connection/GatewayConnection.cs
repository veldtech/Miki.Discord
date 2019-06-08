using System.IO.Compression;
using Miki.Discord.Common.Gateway;
using Miki.Discord.Common.Gateway.Packets;
using Miki.Discord.Gateway.Utils;
using Miki.Logging;
using Miki.Net.WebSockets;
using Miki.Net.WebSockets.Exceptions;
using System;
using System.IO;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Bson;
using System.Collections.Generic;
using Miki.Discord.Common.Converters;

namespace Miki.Discord.Gateway.Connection
{
    public enum ConnectionStatus
    {
        Connecting,
        Connected,
        Identifying,
        Resuming,
        Disconnecting,
        Disconnected,
        Error
    }

    public class GatewayConnection
    {
        public event Func<Task> OnConnect;
        public event Func<Exception, Task> OnDisconnect;
        public event Func<GatewayMessage, Task> OnPacketReceived;

        public ConnectionStatus ConnectionStatus { get; private set; } = ConnectionStatus.Disconnected;

        public int ShardId => _configuration.ShardId;

        public string[] TraceServers { get; private set; }

        private readonly IWebSocketClient _webSocketClient;
        private readonly GatewayProperties _configuration;

        private Task _runTask = null;
        private Task _heartbeatTask = null;

        private int? _sequenceNumber = null;
        private string _sessionId = null;

        private MemoryStream _receiveStream = new MemoryStream(GatewayConstants.WebSocketReceiveSize);
        private byte[] _receivePacket = new byte[GatewayConstants.WebSocketReceiveSize];

        private CancellationTokenSource _connectionToken;
        private SemaphoreSlim _heartbeatLock;

        private MemoryStream _compressedStream;
        private MemoryStream _uncompressStream;
        private StreamReader _uncompressStreamReader;
        private DeflateStream _deflateStream;
        private JsonSerializer _jsonSerializer;

        public bool IsRunning => _runTask != null && !_connectionToken.IsCancellationRequested;

        /// <summary>
        /// Creates a new gateway connection
        /// </summary>
        /// <param name="configuration"></param>
		public GatewayConnection(GatewayProperties configuration)
        {
            if (string.IsNullOrWhiteSpace(configuration.Token))
            {
                throw new ArgumentNullException("Token cannot be empty.");
            }

            _compressedStream = new MemoryStream();
            _uncompressStream = new MemoryStream();
            _uncompressStreamReader = new StreamReader(_uncompressStream, Encoding.UTF8);
            _deflateStream = new DeflateStream(_compressedStream, CompressionMode.Decompress);
            
            _jsonSerializer = JsonSerializer.Create(new
                JsonSerializerSettings
            {
                Converters = new List<JsonConverter>
                {
                    new GatewayMessageConverter()
                }
            });

            _webSocketClient = configuration.WebSocketClientFactory();
            _configuration = configuration;
        }

        public async Task StartAsync()
        {
            // Check all possible statuses before reconnecting.
            if (ConnectionStatus == ConnectionStatus.Connected
                || ConnectionStatus == ConnectionStatus.Connecting
                || ConnectionStatus == ConnectionStatus.Resuming
                || ConnectionStatus == ConnectionStatus.Identifying)
            {
                throw new InvalidOperationException("Shard has already started.");
            }

            ConnectionStatus = ConnectionStatus.Connecting;
            var hello = await InitGateway();
            TraceServers = hello.TraceServers;

            if (_sequenceNumber.HasValue)
            {
                ConnectionStatus = ConnectionStatus.Resuming;
                await ResumeAsync(new GatewayResumePacket
                {
                    Sequence = _sequenceNumber.Value,
                    SessionId = _sessionId,
                    Token = _configuration.Token
                });
            }
            else
            {
                ConnectionStatus = ConnectionStatus.Identifying;
                await IdentifyAsync();
            }

            _heartbeatTask = HeartbeatAsync(hello.HeartbeatInterval);
            _runTask = RunAsync();
            ConnectionStatus = ConnectionStatus.Connected;
        }

        public async Task CloseAsync()
        {
            await StopAsync();
            _sessionId = null;
        }

        public async Task StopAsync()
        {
            ConnectionStatus = ConnectionStatus.Disconnecting;
            if (_connectionToken == null || _runTask == null)
            {
                throw new InvalidOperationException("This gateway client is not running!");
            }

            _connectionToken.Cancel();

            try
            {
                _runTask.Wait();
                _heartbeatTask.Wait();
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }

            // Could be closed already, and will throw a WebsocketException
            try
            {
                await _webSocketClient.CloseAsync(_connectionToken.Token);
            }
            catch (ObjectDisposedException) { /* Means the websocket has been disposed, and is ready to be reused. */ }
            catch (Exception ex)
            {
                Log.Error(ex);
            }

            _connectionToken = null;
            _heartbeatTask = null;
            _runTask = null;
            ConnectionStatus = ConnectionStatus.Disconnected;
        }

        public async Task RunAsync()
        {
            try
            {
                while (!_connectionToken.IsCancellationRequested)
                {
                    var msg = await ReceivePacketAsync()
                        .ConfigureAwait(false);
                    if (!msg.OpCode.HasValue)
                    {
                        continue;
                    }

                    switch (msg.OpCode)
                    {
                        case GatewayOpcode.Dispatch:
                        {
                            _sequenceNumber = msg.SequenceNumber;

                            if (msg.EventName == "READY")
                            {
                                var readyPacket = (msg.Data as JToken)
                                    .ToObject<GatewayReadyPacket>();
                                _sessionId = readyPacket.SessionId;
                                TraceServers = readyPacket.TraceGuilds;
                                _heartbeatLock.Release();
                            }

                            if (msg.EventName == "RESUMED")
                            {
                                var readyPacket = (msg.Data as JToken)
                                    .ToObject<GatewayReadyPacket>();
                                TraceServers = readyPacket.TraceGuilds;
                                _heartbeatLock.Release();
                            }

                            Log.Debug($"    <= {msg.EventName.ToString()}");
                            if (OnPacketReceived != null)
                            {
                                await OnPacketReceived(msg);
                            }
                        }
                        break;

                        case GatewayOpcode.InvalidSession:
                        {
                            var canResume = (msg.Data as JToken)
                                .ToObject<bool>();
                            if (!canResume)
                            {
                                _sequenceNumber = null;
                            }
                            var _ = Task.Run(() => ReconnectAsync());
                        }
                        break;

                        case GatewayOpcode.Reconnect:
                        {
                            var _ = Task.Run(() => ReconnectAsync());
                        }
                        break;

                        case GatewayOpcode.Heartbeat:
                        {
                            await SendHeartbeatAsync();
                        }
                        break;

                        case GatewayOpcode.HeartbeatAcknowledge:
                        {
                            _heartbeatLock.Release();
                        }
                        break;
                    }
                }
            }
            catch (WebSocketException w)
            {
                Log.Error(w);
                _ = ReconnectAsync()
                    .ConfigureAwait(false);
            }
            catch (WebSocketCloseException c)
            {
                Log.Error(c);
                _ = HandleGatewayErrorsAsync(c)
                    .ConfigureAwait(false);
            }
            catch (TaskCanceledException)
            {
                return;
            }
            catch (Exception e)
            {
                ConnectionStatus = ConnectionStatus.Error;
                Log.Error(e);
                await Task.Delay(5000);
                _ = ReconnectAsync();
            }
        }

        private async Task HandleGatewayErrorsAsync(WebSocketCloseException w)
        {
            switch (w.ErrorCode)
            {
                default:
                {
                    await ReconnectAsync()
                        .ConfigureAwait(false);
                }
                break;

                case 4000: // unknown error
                case 4001: // unknown opcode
                case 4002: // decode error
                case 4003: // not authenticated
                case 4004: // authentication failed
                case 4005: // already authenticated
                case 4008: // rate limited
                case 4007: // invalid seq
                case 4009: // session timeout
                {
                    _sequenceNumber = null;
                    await ReconnectAsync()
                        .ConfigureAwait(false);
                }
                break;

                case 4010: // invalid shard
                case 4011: // sharding required
                {
                    await CloseAsync()
                        .ConfigureAwait(false);
                    throw new GatewayException("Websocket returned error that should not be resumed, nor reconnected.", w);
                };
            }
        }

        public async Task HeartbeatAsync(int latency)
        {
            // Will stop running heartbeat if connectionToken is cancelled.
            while (!_connectionToken.IsCancellationRequested)
            {
                try
                {
                    if (!await _heartbeatLock.WaitAsync(latency, _connectionToken.Token))
                    {
                        var _ = Task.Run(() => ReconnectAsync());
                        break;
                    }

                    await SendHeartbeatAsync()
                        .ConfigureAwait(false);

                    await Task.Delay(latency, _connectionToken.Token)
                        .ConfigureAwait(false);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception e)
                {
                    Log.Error(e);
                    break;
                }
            }
        }

        public async Task IdentifyAsync()
        {
            GatewayIdentifyPacket identifyPacket = new GatewayIdentifyPacket
            {
                Compressed = _configuration.Compressed,
                Token = _configuration.Token,
                LargeThreshold = 250,
                Shard = new int[] { _configuration.ShardId, _configuration.ShardCount }
            };

            var canIdentify = await _configuration.Ratelimiter.CanIdentifyAsync()
                .ConfigureAwait(false);
            while (true)
            {
                if (canIdentify)
                {
                    await SendCommandAsync(GatewayOpcode.Identify, identifyPacket, _connectionToken.Token)
                        .ConfigureAwait(false);
                    break;
                }
                else
                {
                    Log.Debug("Could not identify yet, retrying in 5 seconds.");
                    await Task.Delay(5000)
                        .ConfigureAwait(false);
                    canIdentify = await _configuration.Ratelimiter.CanIdentifyAsync()
                        .ConfigureAwait(false);
                }
            }
        }

        private async Task ResumeAsync(GatewayResumePacket packet)
        {
            await SendCommandAsync(GatewayOpcode.Resume, packet, _connectionToken.Token)
                .ConfigureAwait(false);
        }

        public async Task ReconnectAsync(int initialDelay = 1000, bool shouldIncrease = true)
        {
            var delay = initialDelay;
            bool connected = false;

            await StopAsync()
                .ConfigureAwait(false);

            while (!connected)
            {
                try
                {
                    await StartAsync()
                        .ConfigureAwait(false);
                    connected = true;
                }
                catch (Exception e)
                {
                    Log.Error($"Reconnection failed with reason: {e.Message}, will retry in {delay / 1000} seconds");
                    await Task.Delay(delay)
                        .ConfigureAwait(false);
                    if (shouldIncrease)
                    {
                        delay += initialDelay;
                    }
                }
            }
        }

        public async Task SendCommandAsync(GatewayOpcode opcode, object data, CancellationToken token = default)
        {
            GatewayMessage msg = new GatewayMessage
            {
                OpCode = opcode,
                Data = data,
                EventName = null,
                SequenceNumber = null
            };
            await SendCommandAsync(msg, token)
                .ConfigureAwait(false);
        }
        private async Task SendCommandAsync(GatewayMessage msg, CancellationToken token)
        {
            var json = JsonConvert.SerializeObject(msg);

            Log.Debug($"=> {msg.OpCode.ToString()}");
            Log.Trace($"    json packet: {json}");

            await _webSocketClient.SendAsync(json, token)
                .ConfigureAwait(false);
        }

        private async Task SendHeartbeatAsync()
        {
            GatewayMessage msg = new GatewayMessage
            {
                OpCode = GatewayOpcode.Heartbeat,
                Data = _sequenceNumber
            };
            await SendCommandAsync(msg, _connectionToken.Token)
                .ConfigureAwait(false);
        }

        private async Task<GatewayHelloPacket> InitGateway()
        {
            _heartbeatLock = new SemaphoreSlim(0, 1);
            _connectionToken = new CancellationTokenSource();

            string connectionUri = new WebSocketUrlBuilder("wss://gateway.discord.gg/")
                .SetCompression(_configuration.Compressed)
                .SetEncoding(_configuration.Encoding)
                .SetVersion(_configuration.Version)
                .Build();

            await _webSocketClient.ConnectAsync(new Uri(connectionUri), _connectionToken.Token);
            var msg = await ReceivePacketAsync();
            return (msg.Data as JToken)
                .ToObject<GatewayHelloPacket>();
        }

        private async Task<WebSocketPacket> ReceivePacketBytesAsync()
        {
            _receiveStream.Position = 0;
            _receiveStream.SetLength(0);

            WebSocketResponse response;
            do
            {
                if (_connectionToken.IsCancellationRequested)
                {
                    throw new OperationCanceledException();
                }

                response = await _webSocketClient.ReceiveAsync(
                    new ArraySegment<byte>(_receivePacket), _connectionToken.Token)
                    .ConfigureAwait(false);

                if (response.Count + _receiveStream.Position > _receiveStream.Capacity)
                {
                    _receiveStream.Capacity = _receiveStream.Capacity * 2;
                }

                await _receiveStream.WriteAsync(_receivePacket, 0, response.Count, _connectionToken.Token)
                    .ConfigureAwait(false);
            }
            while (!response.EndOfMessage);

            response.Count = (int)_receiveStream.Position;
            Memory<byte> p = _receiveStream.GetBuffer();

            return new WebSocketPacket(response, p);
        }

        private async Task<GatewayMessage> ReceivePacketAsync()
        {
            var response = await ReceivePacketBytesAsync()
                .ConfigureAwait(false);

            _uncompressStream.SetLength(0);
            _uncompressStream.Position = 0;

            if (_configuration.Compressed)
            {
                if (response.Packet.Span[0] == 0x78)
                {
                    //Strip the zlib header
                    _compressedStream.Write(response.Packet.ToArray(), 2, response.Response.Count - 2);
                    _compressedStream.SetLength(response.Response.Count - 2);
                }
                else
                {
                    _compressedStream.Write(response.Packet.ToArray(), 0, response.Response.Count);
                    _compressedStream.SetLength(response.Response.Count);
                }

                _compressedStream.Position = 0;
                await _deflateStream.CopyToAsync(_uncompressStream);
                _compressedStream.Position = 0;
            }
            else
            {
                _uncompressStream.Write(response.Packet.ToArray(), 0, response.Response.Count);
                _uncompressStream.SetLength(response.Response.Count);
            }

            _uncompressStream.Position = 0;

            if (_configuration.Encoding == GatewayEncoding.Json)
            {
                var msg = (GatewayMessage)_jsonSerializer
                    .Deserialize(_uncompressStreamReader, typeof(GatewayMessage));
                return msg;
            }
            else
            {
                return new GatewayMessage{ };
            }
        }
    }

    public struct GatewayPacket
    {
        public GatewayMessage Message;
        public WebSocketPacket Packet;
    }

	public struct WebSocketPacket
	{
		public WebSocketResponse Response { get; }

        public Memory<byte> Packet { get; }

		public WebSocketPacket(WebSocketResponse r, Memory<byte> packet)
		{
			Packet = packet;
			Response = r;
		}
	}
}