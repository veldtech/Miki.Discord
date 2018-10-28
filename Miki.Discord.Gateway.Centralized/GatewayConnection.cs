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

		private CancellationTokenSource _connectionToken;

		public bool IsRunning => _runTask != null && !_connectionToken.IsCancellationRequested;

		public GatewayConnection(GatewayConfiguration configuration)
		{
			WebSocketClient = configuration.WebSocketClient ?? new BasicWebSocketClient();
			_configuration = configuration;
		}

		public async Task StartAsync()
		{
			_connectionToken = new CancellationTokenSource();

			var connectionProperties = await _configuration.ApiClient.GetGatewayBotAsync();

			string connectionUri = new WebSocketUrlBuilder(connectionProperties.Url)
				.SetCompression(_configuration.Compressed)
				.SetEncoding(_configuration.Encoding)
				.SetVersion(_configuration.Version)
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
			identifyPacket.Compressed = _configuration.Compressed;
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

		private async Task<WebSocketResponse> ReceivePacketBytesAsync(byte[] packet)
		{
			MemoryStream _receiveStream = new MemoryStream(GatewayConstants.WebSocketReceiveSize);

			_receiveStream.Position = 0;
			_receiveStream.SetLength(0);

			WebSocketResponse response;
			do
			{
				if (_connectionToken.IsCancellationRequested)
				{
					throw new OperationCanceledException();
				}

				response = await WebSocketClient.ReceiveAsync(new ArraySegment<byte>(packet), _connectionToken.Token);
				await _receiveStream.WriteAsync(packet, 0, response.Count);
			}
			while (!response.EndOfMessage);

			packet = _receiveStream.TryGetBuffer(out var responseBuffer) ? responseBuffer.Array : _receiveStream.ToArray();
			return response;
		}

		private async Task<GatewayMessage> ReceivePacketAsync()
		{
			byte[] packet = new byte[GatewayConstants.WebSocketReceiveSize];
			WebSocketResponse response = await ReceivePacketBytesAsync(packet);

			if (response.MessageType == WebSocketContentType.Text)
			{
				string textPacket = Encoding.UTF8.GetString(packet, 0, response.Count);

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
}