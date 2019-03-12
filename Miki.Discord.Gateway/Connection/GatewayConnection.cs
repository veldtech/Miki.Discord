using System.IO.Compression;
using Miki.Discord.Common.Gateway;
using Miki.Discord.Common.Gateway.Packets;
using Miki.Discord.Gateway.Utils;
using Miki.Logging;
using Miki.Net.WebSockets;
using Miki.Net.WebSockets.Exceptions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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
        public event Func<GatewayMessage, ArraySegment<byte>, Task> OnPacketReceived;

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

        private JsonSerializer _serializer;

        private MemoryStream _compressedStream;
        private DeflateStream _deflateStream;

        public bool IsRunning => _runTask != null && !_connectionToken.IsCancellationRequested;

        /// <summary>
        /// Creates a new gateway connection
        /// </summary>
        /// <param name="configuration"></param>
		public GatewayConnection(GatewayProperties configuration)
		{
			if(string.IsNullOrWhiteSpace(configuration.Token))
			{
				throw new ArgumentNullException("Token cannot be empty.");
			}

            _compressedStream = new MemoryStream();
            _deflateStream = new DeflateStream(_compressedStream, CompressionMode.Decompress);
            _serializer = new JsonSerializer();

			_webSocketClient = configuration.WebSocketClient 
                ?? new BasicWebSocketClient();

			_configuration = configuration;
		}

		public async Task StartAsync()
		{
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

            // Could be closed already, and will throw a WebsocketException
            try
            {
                await _webSocketClient.CloseAsync(_connectionToken.Token);
            }
            catch(Exception ex)
            {
                Log.Error(ex);
            }

            try
            {
                _runTask.Wait();
                _heartbeatTask.Wait();
            }
            catch(Exception ex)
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
                    var (response, bytes) = await ReceivePacketAsync();
                    if(response == null)
                    {
                        continue;
                    }

                    switch (response.OpCode)
                    {
                        case GatewayOpcode.Dispatch:
                        {
                            _sequenceNumber = response.SequenceNumber;

                            if (response.EventName == "READY")
                            {
                                var readyPacket = (response.Data as JToken)
                                    .ToObject<GatewayReadyPacket>();
                                _sessionId = readyPacket.SessionId;
                                TraceServers = readyPacket.TraceGuilds;
                                _heartbeatLock.Release();
                            }

                            if (response.EventName == "RESUMED")
                            {
                                var readyPacket = (response.Data as JToken)
                                    .ToObject<GatewayReadyPacket>();
                                TraceServers = readyPacket.TraceGuilds;
                                _heartbeatLock.Release();
                            }

                            Log.Debug($"    <= {response.EventName.ToString()}");
                            if (OnPacketReceived != null)
                            {
                                await OnPacketReceived(response, new ArraySegment<byte>(bytes));
                            }
                        } break;

                        case GatewayOpcode.InvalidSession:
                        {
                            var canResume = (response.Data as JToken)
                                .ToObject<bool>();
                            if(!canResume)
                            {
                                _sequenceNumber = null;
                            }
                            var _ = Task.Run(() => ReconnectAsync());
                        } break;

                        case GatewayOpcode.Reconnect:
                        {
                            var _ = Task.Run(() => ReconnectAsync());
                        } break;

                        case GatewayOpcode.Heartbeat:
                        {
                            await SendHeartbeatAsync();
                        } break;

                        case GatewayOpcode.HeartbeatAcknowledge:
                        {
                            _heartbeatLock.Release();
                        } break; 
                    }
                }
            }
            catch(WebSocketException w)
            {                
                Log.Error(w);
                var _ = Task.Run(() => ReconnectAsync());
            }
            catch(WebSocketCloseException c)
            {
                Log.Error(c);
                var _ = Task.Run(() => HandleGatewayErrorsAsync(c));
            }
            catch(TaskCanceledException _)
            {
                return;
            }
            catch(Exception e)
            {
                ConnectionStatus = ConnectionStatus.Error;
                Log.Error(e);
                await Task.Delay(5000);
                var _ = Task.Run(() => ReconnectAsync());
            }
        }

        private async Task HandleGatewayErrorsAsync(WebSocketCloseException w)
        {
            switch(w.ErrorCode)
            {
                default:
                {
                    await ReconnectAsync();
                } break;

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
                    await ReconnectAsync();
                } break;

                case 4010: // invalid shard
                case 4011: // sharding required
                {
                    await CloseAsync();
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
                    if(!await _heartbeatLock.WaitAsync(latency, _connectionToken.Token))
                    {
                        var _ = Task.Run(() => ReconnectAsync());
                        break;
                    }

                    await SendHeartbeatAsync();

                    await Task.Delay(latency, _connectionToken.Token);
                }
                catch(Exception e)
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

            var canIdentify = await _configuration.Ratelimiter.CanIdentifyAsync();
            while (true)
            {
                if (canIdentify)
                {
                    await SendCommandAsync(GatewayOpcode.Identify, identifyPacket, _connectionToken.Token);
                    break;
                }
                else
                {
                    Log.Debug("Could not identify yet, retrying in 5 seconds.");
                    await Task.Delay(5000);
                    canIdentify = await _configuration.Ratelimiter.CanIdentifyAsync();
                }
            }
		}

        private async Task ResumeAsync(GatewayResumePacket packet)
        {
            await SendCommandAsync(GatewayOpcode.Resume, packet, _connectionToken.Token);
        }

        public async Task ReconnectAsync(int initialDelay = 1000, bool shouldIncrease = true)
        {
            var delay = initialDelay;
            bool connected = false;
            await StopAsync();
            while (!connected)
            {
                try
                {
                    await StartAsync();
                    connected = true;
                }
                catch (Exception e)
                {
                    Log.Error($"Reconnection failed with reason: {e.Message}, will retry in {delay/1000} seconds");
                    await Task.Delay(delay);
                    if (shouldIncrease)
                    {
                        delay += initialDelay;
                    }
                }
            }
        }

        public async Task SendCommandAsync(GatewayOpcode opcode, object data, CancellationToken token = default(CancellationToken))
        {
            GatewayMessage msg = new GatewayMessage
            {
                OpCode = opcode,
                Data = data,
                EventName = null,
                SequenceNumber = null
            };
            await SendCommandAsync(msg, token);
		}
        private async Task SendCommandAsync(GatewayMessage msg, CancellationToken token)
        {
            var json = JsonConvert.SerializeObject(msg);

            Log.Debug($"=> {msg.OpCode.ToString()}");
            Log.Trace($"    json packet: {json}");

            await _webSocketClient.SendAsync(json, token);
        }

        private async Task SendHeartbeatAsync()
        {
            GatewayMessage msg = new GatewayMessage
            {
                OpCode = GatewayOpcode.Heartbeat,
                Data = _sequenceNumber
            };
            await SendCommandAsync(msg, _connectionToken.Token);
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
            var (response, _) = await ReceivePacketAsync();
            return (response.Data as JToken).ToObject<GatewayHelloPacket>();
        }

        private async Task<WebSocketPacket> ReceivePacketBytesAsync()
		{
			int size = 0;
			_receiveStream.Position = 0;
			_receiveStream.SetLength(0);

			WebSocketResponse response;
			do
			{
				if (_connectionToken.IsCancellationRequested)
				{
					throw new OperationCanceledException();
				}

				response = await _webSocketClient.ReceiveAsync(new ArraySegment<byte>(_receivePacket), _connectionToken.Token);
				size += response.Count;

				if (response.Count + _receiveStream.Position > _receiveStream.Capacity)
				{
					_receiveStream.Capacity = _receiveStream.Capacity * 2;
				}

				await _receiveStream.WriteAsync(_receivePacket, 0, response.Count, _connectionToken.Token);
			}
			while (!response.EndOfMessage);

			byte[] p = _receiveStream.TryGetBuffer(out var responseBuffer) 
                ? responseBuffer.Array 
                : _receiveStream.ToArray();

			response.Count = size;

			return new WebSocketPacket(response, p);
		}       

        private bool IsZlibPacket(byte[] b, int count)
        {
            return b[count - 4] == '\x00'
                && b[count - 3] == '\x00'
                && b[count - 2] == '\xff'
                && b[count - 1] == '\xff';
        }

		private async Task<(GatewayMessage, byte[])> ReceivePacketAsync()
		{
			var response = await ReceivePacketBytesAsync();
            using (var memoryStream = new MemoryStream())
            {
                if (_configuration.Compressed)
                {
                    if (response.Packet[0] == 0x78)
                    {
                        //Strip the zlib header
                        _compressedStream.Write(response.Packet, 2, response.Response.Count - 2);
                        _compressedStream.SetLength(response.Response.Count - 2);
                    }
                    else
                    {
                        _compressedStream.Write(response.Packet, 0, response.Response.Count);
                        _compressedStream.SetLength(response.Response.Count);
                    }

                    _compressedStream.Position = 0;
                    await _deflateStream.CopyToAsync(memoryStream);
                    _compressedStream.Position = 0;
                    memoryStream.Position = 0;
                }
                else
                {
                    memoryStream.Write(response.Packet, 0, response.Response.Count);
                    memoryStream.SetLength(response.Response.Count);
                }

                if (_configuration.Encoding == GatewayEncoding.Json)
                {
                    using (var stringReader = new StreamReader(memoryStream))
                    using (var jsonReader = new JsonTextReader(stringReader))
                    {
                        var msg = _serializer.Deserialize<GatewayMessage>(jsonReader);
                        if (msg == null)
                        {
                            return (null, null);
                        }
                        return (msg, memoryStream.GetBuffer());
                    }
                }
                else
                {
                    // TODO (Veld): Add ETF
                    throw new NotSupportedException();
                }
            }
        }
	}

	public class WebSocketPacket
	{
		public WebSocketResponse Response { get; }
		public byte[] Packet { get; }

		public WebSocketPacket(WebSocketResponse r, byte[] packet)
		{
			Packet = packet;
			Response = r;
		}
	}
}