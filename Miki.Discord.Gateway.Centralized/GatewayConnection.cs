using Miki.Discord.Common.Gateway;
using Miki.Discord.Common.Gateway.Packets;
using Miki.Discord.Gateway.Centralized.Utils;
using Miki.Logging;
using Miki.Net.WebSockets;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Miki.Discord.Gateway.Centralized
{
	public class GatewayConnection
	{
        public event Func<Task> OnConnect;
        public event Func<Exception, Task> OnDisconnect; 
        public event Func<GatewayMessage, Task> OnPacketReceived;

		public readonly IWebSocketClient WebSocketClient;

		private GatewayProperties _configuration;

		private Task _runTask = null;
		private Task _heartbeatTask = null;

		private int? _sequenceNumber = null;
        private string _sessionId = null;
		private MemoryStream _receiveStream = new MemoryStream(GatewayConstants.WebSocketReceiveSize);
		private byte[] _receivePacket = new byte[GatewayConstants.WebSocketReceiveSize];

		private CancellationTokenSource _connectionToken;
        private SemaphoreSlim _heartbeatLock;

		public bool IsRunning => _runTask != null && !_connectionToken.IsCancellationRequested;

        /// <summary>
        /// Creates a new gateway connection
        /// </summary>
        /// <param name="configuration"></param>
		public GatewayConnection(GatewayProperties configuration)
		{
			if(string.IsNullOrWhiteSpace(configuration.Token))
			{
				throw new ArgumentNullException("Token can not be empty.");
			}

			WebSocketClient = configuration.WebSocketClient 
                ?? new BasicWebSocketClient();

			_configuration = configuration;
		}

		public async Task StartAsync()
		{
            await InitGateway();

            var response = await ReceivePacketAsync();

			var helloPacket = (response.Data as JToken)
                .ToObject<GatewayHelloPacket>();

			await IdentifyAsync(helloPacket);

			_heartbeatTask = HeartbeatAsync(helloPacket.HeartbeatInterval);
			_runTask = RunAsync();
		}

        public async Task CloseAsync()
        {
            await StopAsync();
            _sessionId = null;        
        }

        public async Task StopAsync()
		{
			if (_connectionToken == null || _runTask == null)
			{
				throw new InvalidOperationException("This gateway client is not running!");
			}

			_connectionToken.Cancel();

            // Could be closed already, and will throw a WebsocketException
            try
            {
                await WebSocketClient.CloseAsync(_connectionToken.Token);
            }
            catch { }

            _runTask.Wait();
            _heartbeatTask.Wait();

            _connectionToken = null;
            _heartbeatTask = null;
			_runTask = null;
		}

        public async Task RunAsync()
        {
            try
            {
                while (!_connectionToken.IsCancellationRequested)
                {
                    var response = await ReceivePacketAsync();

                    if (response.Data == null)
                    {
                        continue;
                    }

                    _sequenceNumber = response.SequenceNumber;

                    switch (response.OpCode)
                    {
                        case GatewayOpcode.Dispatch:
                        {
                            if(response.EventName == "READY")
                            {
                                var readyPacket = (response.Data as JToken)
                                    .ToObject<GatewayReadyPacket>();
                                _sessionId = readyPacket.SessionId;
                                _heartbeatLock.Release();
                            }

                            Log.Debug($"<= {response.EventName.ToString()}");
                            if (OnPacketReceived != null)
                            {
                                await OnPacketReceived(response);
                            }
                        } break;

                        case GatewayOpcode.Heartbeat:
                        {
                            await SendHeartbeatAsync();
                        } break;

                        case GatewayOpcode.HeartbeatAcknowledge:
                        {
                            Log.Debug("Heartbeat ACK received!");
                            _heartbeatLock.Release();
                        } break; 
                    }
                }
            }
            catch(WebSocketException w)
            {

            }
        }

        public async Task HeartbeatAsync(int latency)
        {
            // Will stop running heartbeat if connectionToken is broken.
            while (!_connectionToken.IsCancellationRequested)
            {
                try
                { 
                    await SendHeartbeatAsync();
                    await Task.Delay(latency);
                }
                catch
                {
                    await ResumeAsync(new GatewayResumePacket
                    {
                        Sequence = _sequenceNumber ?? 0,
                        SessionId = _sessionId,
                        Token = _configuration.Token
                    });
                }
            }   
        }

		public async Task IdentifyAsync(GatewayHelloPacket packet)
		{
            GatewayIdentifyPacket identifyPacket = new GatewayIdentifyPacket
            {
                Compressed = _configuration.Compressed.GetValueOrDefault(false),
                ConnectionProperties = new GatewayIdentifyConnectionProperties
                {
                    Browser = "Miki.Discord",
                    OperatingSystem = Environment.OSVersion.ToString(),
                    Device = "Miki.Discord",
                },
                Token = _configuration.Token,
                LargeThreshold = 250,
                Shard = new int[] { _configuration.ShardId, _configuration.ShardCount }
            };

			await SendCommandAsync(GatewayOpcode.Identify, identifyPacket, _connectionToken.Token);
		}

        public async Task ResumeAsync(GatewayResumePacket packet)
        {
            try
            {
                await StopAsync();
                await InitGateway();
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
            var response = await ReceivePacketAsync();

            var helloPacket = (response.Data as JToken)
                .ToObject<GatewayHelloPacket>();

            await SendCommandAsync(GatewayOpcode.Resume, packet, _connectionToken.Token);
        }

        public async Task SendCommandAsync(GatewayOpcode opcode, object data, CancellationToken token)
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
            var json = JsonConvert.SerializeObject(msg, GatewayConstants.JsonSettings);

            Log.Debug($"=> {msg.OpCode.ToString()}");

            await WebSocketClient.SendAsync(json, token);

        }

        private async Task SendHeartbeatAsync()
        {
            GatewayMessage msg = new GatewayMessage
            {
                SequenceNumber = _sequenceNumber
            };
            await SendCommandAsync(GatewayOpcode.Heartbeat, msg, _connectionToken.Token);
            Log.Debug("Heartbeat sent!");
        }

        private async Task InitGateway()
        {
            _heartbeatLock = new SemaphoreSlim(0, 1);
            _connectionToken = new CancellationTokenSource();

            string connectionUri = new WebSocketUrlBuilder("wss://gateway.discord.gg/")
                .SetCompression(_configuration.Compressed.GetValueOrDefault(false))
                .SetEncoding(_configuration.Encoding.GetValueOrDefault(GatewayEncoding.Json))
                .SetVersion(_configuration.Version.GetValueOrDefault(GatewayConstants.DefaultVersion))
                .Build();

            await WebSocketClient.ConnectAsync(new Uri(connectionUri), _connectionToken.Token);
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

				response = await WebSocketClient.ReceiveAsync(new ArraySegment<byte>(_receivePacket), _connectionToken.Token);
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
		private async Task<GatewayMessage> ReceivePacketAsync()
		{
			var response = await ReceivePacketBytesAsync();

			if (response.Response.MessageType == WebSocketContentType.Text)
			{
				string textPacket = Encoding.UTF8.GetString(response.Packet, 0, response.Response.Count);
                return JsonConvert.DeserializeObject<GatewayMessage>(
                    textPacket,
                    GatewayConstants.JsonSettings);
			}
			else
			{
				// TODO (Veld): Add zlib && ETF
				return null;
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