using Miki.Discord.Common;
using Miki.Discord.Common.Events;
using Miki.Discord.Common.Packets;
using Miki.Discord.Gateway.Centralized.Utils;
using Miki.Discord.Rest.Entities;
using Miki.Net.WebSockets;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Miki.Discord.Gateway.Centralized
{
	/// <summary>
	/// Discord.NET "influenced" (most of it.) websocket implementation.
	/// </summary>
	public class GatewayConnection
	{
		public Func<string, Task> OnTextPacket { get; set; }
		public Func<byte[], Task> OnBinaryPacket { get; set; }

		public readonly IWebSocketClient WebSocketClient;

		GatewayConfiguration _configuration;

		Task _runTask = null;

		CancellationTokenSource _connectionToken;

		MemoryStream _receiveStream = new MemoryStream(GatewayConstants.WebSocketReceiveSize);
		MemoryStream _sendStream = new MemoryStream(GatewayConstants.WebSocketSendSize);

		public bool IsRunning => _runTask != null && !_connectionToken.IsCancellationRequested;

		public GatewayConnection(GatewayConfiguration configuration)
		{
			WebSocketClient = configuration.WebSocketClient ?? new BasicWebSocketClient();
			_configuration = configuration;
		}

		public async Task StartAsync()
		{
			_connectionToken = new CancellationTokenSource();

			string connectionUri = WebsocketUrlBuilder.FromGatewayConfiguration(_configuration);

			await WebSocketClient.ConnectAsync(new Uri(connectionUri), _connectionToken.Token);

			_runTask = RunAsync();
		}

		public async Task StopAsync()
		{
			if(_connectionToken == null || _runTask == null)
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
					_receiveStream.Position = 0;
					_receiveStream.SetLength(0);

					byte[] buffer = new byte[GatewayConstants.WebSocketReceiveSize];
					byte[] packet;
					int packetSize;

					WebSocketResponse response;
					do
					{
						if (_connectionToken.IsCancellationRequested)
						{
							return;
						}

						response = await WebSocketClient.ReceiveAsync(buffer, _connectionToken.Token);
						_receiveStream.Write(buffer, 0, response.Count);
					}
					while (!response.EndOfMessage);

					packet = _receiveStream.TryGetBuffer(out var responseBuffer) ? responseBuffer.Array : _receiveStream.ToArray();
					packetSize = (int)_receiveStream.Length;

					if (response.MessageType == WebSocketContentType.Text)
					{
						string textPacket = Encoding.UTF8.GetString(packet, 0, packetSize);

						if (OnTextPacket != null)
						{
							await OnTextPacket(textPacket);
						}
					}
					else
					{
						// TODO (Veld): Handle Message Binary
					}
				}
			}
			catch (Exception e)
			{
				Console.WriteLine(e.ToString());
			}
		}
	}
}
