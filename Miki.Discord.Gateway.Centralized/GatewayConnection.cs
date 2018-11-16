using Miki.Discord.Common.Gateway;
using Miki.Discord.Common.Gateway.Packets;
using Miki.Discord.Gateway.Centralized.Utils;
using Miki.Logging;
using Miki.Net.WebSockets;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Miki.Discord.Gateway.Centralized
{
	public class GatewayConnection
	{
		public Func<GatewayMessage, Task> OnPacketReceived { get; set; }
		public Func<byte[], Task> OnBinaryPacket { get; set; }

		public readonly IWebSocketClient WebSocketClient;

		private GatewayConfiguration _configuration;

		private Task _runTask = null;
		private Task _heartbeatTask = null;

		private int? _sequenceNumber = null;
		private MemoryStream _receiveStream = new MemoryStream(GatewayConstants.WebSocketReceiveSize);
		private byte[] _receivePacket = new byte[GatewayConstants.WebSocketReceiveSize];

		private CancellationTokenSource _connectionToken;

		public bool IsRunning => _runTask != null && !_connectionToken.IsCancellationRequested;

		public GatewayConnection(GatewayConfiguration configuration)
		{
			if(string.IsNullOrWhiteSpace(configuration.Token))
			{
				throw new ArgumentNullException("Token can not be blank.");
			}

			WebSocketClient = configuration.WebSocketClient ?? new BasicWebSocketClient();
			_configuration = configuration;
		}

		public async Task StartAsync()
		{
			_connectionToken = new CancellationTokenSource();

			string connectionUri = new WebSocketUrlBuilder("wss://gateway.discord.gg/")
				.SetCompression(_configuration.Compressed.GetValueOrDefault(false))
				.SetEncoding(_configuration.Encoding.GetValueOrDefault(GatewayEncoding.Json))
				.SetVersion(_configuration.Version.GetValueOrDefault(GatewayConstants.DefaultVersion))
				.Build();

			await WebSocketClient.ConnectAsync(new Uri(connectionUri), _connectionToken.Token);

			var response = await ReceivePacketAsync();

			var helloPacket = (response.Data as JToken).ToObject<GatewayHelloPacket>();
			await IdentifyAsync(helloPacket);

			_heartbeatTask = HeartbeatAsync(helloPacket.HeartbeatInterval);
			_runTask = RunAsync();
		}

		public async Task StopAsync()
		{
			if (_connectionToken == null || _runTask == null)
			{
				throw new InvalidOperationException("This gateway client is not running!");
			}

			_connectionToken.Cancel();

			await _runTask;
			await WebSocketClient.CloseAsync(_connectionToken.Token);

			_connectionToken = null;
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

					if (OnPacketReceived != null)
					{
						await OnPacketReceived(response);
					}
				}
			}
			catch (Exception e)
			{
				Console.WriteLine("error: " + e.ToString());
			}
		}

		public async Task HeartbeatAsync(int latency)
		{
			GatewayMessage msg = new GatewayMessage();

			while (!_connectionToken.IsCancellationRequested)
			{
				await Task.Delay(latency);
				msg.SequenceNumber = _sequenceNumber;
				await SendCommandAsync(GatewayOpcode.Heartbeat, msg, _connectionToken.Token);
			}
		}

		public async Task IdentifyAsync(GatewayHelloPacket packet)
		{
			GatewayIdentifyPacket identifyPacket = new GatewayIdentifyPacket();
			identifyPacket.Compressed = _configuration.Compressed.GetValueOrDefault(false);
			identifyPacket.ConnectionProperties = new GatewayIdentifyConnectionProperties
			{
				Browser = "Miki.Discord",
				OperatingSystem = Environment.OSVersion.ToString(),
				Device = "Miki.Discord",
			};
			identifyPacket.Token = _configuration.Token;

			identifyPacket.LargeThreshold = 250;
			identifyPacket.Shard = new int[] { _configuration.ShardId, _configuration.ShardCount };

			CancellationTokenSource token = new CancellationTokenSource();

			await SendCommandAsync(GatewayOpcode.Identify, identifyPacket, token.Token);
		}

		public async Task SendCommandAsync(GatewayOpcode opcode, object data, CancellationToken token)
		{
			GatewayMessage msg = new GatewayMessage();

			msg.OpCode = opcode;
			msg.Data = data;
			msg.EventName = null;
			msg.SequenceNumber = null;

			var json = JsonConvert.SerializeObject(msg, GatewayConstants.JsonSettings);

			Log.Debug($"=> {msg.OpCode.ToString()} | {json}");

			await WebSocketClient.SendAsync(json, token);
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

				await _receiveStream.WriteAsync(_receivePacket, 0, response.Count);
			}
			while (!response.EndOfMessage);

			byte[] p = _receiveStream.TryGetBuffer(out var responseBuffer) ? responseBuffer.Array : _receiveStream.ToArray();
			response.Count = size;

			return new WebSocketPacket(response, p);
		}

		private async Task<GatewayMessage> ReceivePacketAsync()
		{
			var response = await ReceivePacketBytesAsync();

			if (response.Response.MessageType == WebSocketContentType.Text)
			{
				string textPacket = Encoding.UTF8.GetString(response.Packet, 0, response.Response.Count);

				GatewayMessage msg = JsonConvert.DeserializeObject<GatewayMessage>(textPacket, GatewayConstants.JsonSettings);

				_sequenceNumber = msg.SequenceNumber;

				Log.Debug($"<= {msg.OpCode.ToString()} | {textPacket}");

				return msg;
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