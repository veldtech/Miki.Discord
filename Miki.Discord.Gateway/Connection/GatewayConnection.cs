using System.IO.Compression;
using Miki.Discord.Common.Gateway;
using Miki.Discord.Common.Gateway.Packets;
using Miki.Discord.Gateway.Utils;
using Miki.Logging;
using Miki.Net.WebSockets;
using Miki.Net.WebSockets.Exceptions;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Miki.Discord.Common.Extensions;
using Miki.Serialization;

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
        public event Func<GatewayMessageIdentifier, byte[], Task> Dispatch;

        public ConnectionStatus ConnectionStatus { get; private set; } = ConnectionStatus.Disconnected;

        public int ShardId => _configuration.ShardId;

        private readonly IWebSocketClient _webSocketClient;
        private readonly GatewayProperties _configuration;

        private Task _runTask = null;
        private Task _heartbeatTask = null;

        private string _sessionId = null;

        private MemoryStream _receiveStream = new MemoryStream(GatewayConstants.WebSocketReceiveSize);
        private byte[] _receivePacket = new byte[GatewayConstants.WebSocketReceiveSize];

        private CancellationTokenSource _connectionToken;
        private SemaphoreSlim _heartbeatLock;

        private MemoryStream _compressedStream;
        private MemoryStream _uncompressStream;
        private StreamReader _uncompressStreamReader;
        private DeflateStream _deflateStream;
        private IJsonSerializer _jsonSerializer;

        public bool IsRunning => _runTask != null && !_connectionToken.IsCancellationRequested;

        internal int? SequenceNumber;

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
            _jsonSerializer = configuration.JsonSerializer;
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

            _heartbeatLock = new SemaphoreSlim(0, 1);
            _connectionToken = new CancellationTokenSource();

            var connectionUri = new WebSocketUrlBuilder("wss://gateway.discord.gg/")
                .SetCompression(_configuration.Compressed)
                .SetEncoding(_configuration.Encoding)
                .SetVersion(_configuration.Version)
                .Build();

            await _webSocketClient.ConnectAsync(new Uri(connectionUri), _connectionToken.Token);

            if (SequenceNumber.HasValue)
            {
                ConnectionStatus = ConnectionStatus.Resuming;
                await ResumeAsync(new GatewayResumePacket
                {
                    Sequence = SequenceNumber.Value,
                    SessionId = _sessionId,
                    Token = _configuration.Token
                });
            }
            else
            {
                ConnectionStatus = ConnectionStatus.Identifying;
                await IdentifyAsync();
            }

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
                    var data = await ReceivePacketAsync().ConfigureAwait(false);

                    await HandleMessageAsync(data).ConfigureAwait(false);
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
                // ignore.
            }
            catch (Exception e)
            {
                ConnectionStatus = ConnectionStatus.Error;
                Log.Error(e);
                await Task.Delay(5000);
                _ = ReconnectAsync();
            }
        }

        private Task HandleMessageAsync(byte[] data)
        {
            var identifier = GatewayMessageIdentifier.Read(data);

            if (!identifier.OpCode.HasValue)
            {
                Log.Warning($"Received a message that could not be read by the gateway: {Encoding.UTF8.GetString(data)}");
                return Task.CompletedTask;
            }

            switch (identifier.OpCode)
            {
                case GatewayOpcode.Dispatch:
                    if (identifier.EventName == "READY" || identifier.EventName == "RESUMED")
                    {
                        _heartbeatLock.Release();
                    }

                    return Dispatch.InvokeAsync(identifier, data);

                case GatewayOpcode.Heartbeat:
                    return SendHeartbeatAsync();

                case GatewayOpcode.HeartbeatAcknowledge:
                    _heartbeatLock.Release();
                    return Task.CompletedTask;

                case GatewayOpcode.Hello:
                {
                    var packet = _jsonSerializer.Deserialize<GatewayMessage<GatewayHelloPacket>>(data);
                    _heartbeatTask = HeartbeatAsync(packet.Data.HeartbeatInterval);
                    return Task.CompletedTask;
                }

                case GatewayOpcode.InvalidSession:
                {
                    var canResume = _jsonSerializer.Deserialize<GatewayMessage<bool>>(data).Data;

                    if (!canResume)
                    {
                        SequenceNumber = null;
                    }

                    Log.Trace("Got invalid session, reconnecting...");
                    _ = Task.Run(() => ReconnectAsync());
                    return Task.CompletedTask;
                }

                default:
                    Log.Trace($"Unhandled opcode {identifier.OpCode}");
                    return Task.CompletedTask;
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
                    SequenceNumber = null;
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
                Shard = new[] { _configuration.ShardId, _configuration.ShardCount }
            };

            var canIdentify = await _configuration.Ratelimiter.CanIdentifyAsync()
                .ConfigureAwait(false);

            while (!canIdentify)
            {
                Log.Debug("Could not identify yet, retrying in 5 seconds.");

                await Task.Delay(5000)
                    .ConfigureAwait(false);

                canIdentify = await _configuration.Ratelimiter.CanIdentifyAsync()
                    .ConfigureAwait(false);
            }

            await SendCommandAsync(GatewayOpcode.Identify, identifyPacket, _connectionToken.Token)
                .ConfigureAwait(false);
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

        public async Task SendCommandAsync<T>(GatewayOpcode opcode, T data, CancellationToken token = default)
        {
            var msg = new GatewayMessage<T>
            {
                OpCode = opcode,
                Data = data,
                EventName = null,
                SequenceNumber = null
            };
            await SendCommandAsync(msg, token)
                .ConfigureAwait(false);
        }
        private async Task SendCommandAsync<T>(GatewayMessage<T> msg, CancellationToken token)
        {
            var json = JsonConvert.SerializeObject(msg);

            Log.Debug($"=> {msg.OpCode.ToString()}");
            Log.Trace($"    json packet: {json}");

            await _webSocketClient.SendAsync(json, token)
                .ConfigureAwait(false);
        }

        private async Task SendHeartbeatAsync()
        {
            var msg = new GatewayMessage<int?>
            {
                OpCode = GatewayOpcode.Heartbeat,
                Data = SequenceNumber
            };
            await SendCommandAsync(msg, _connectionToken.Token)
                .ConfigureAwait(false);
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

        private async Task<byte[]> ReceivePacketAsync()
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

            if (_configuration.Encoding != GatewayEncoding.Json)
            {
                throw new NotSupportedException();
            }

            return _uncompressStream.ToArray();
        }
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