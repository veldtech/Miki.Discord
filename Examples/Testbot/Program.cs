using Miki.Discord.Gateway.Centralized;
using Miki.Net.WebSockets;
using Miki.Discord.Common.Gateway.Packets;
using System;
using System.Threading.Tasks;
using Miki.Discord.Common.Gateway;

namespace Testbot
{
    class Program
    {
        static async Task Main(string[] args)
        {
			var client = new CentralizedGatewayShard(new GatewayConfiguration
			{
				Token = "TOKEN HERE",
				Compressed = false,
				Encoding = GatewayEncoding.Json,
				Version = 6,
				WebSocketClient = new BasicWebSocketClient()
			});

			client.OnPacketSent += async (message) =>
			{
				Console.WriteLine($"< [{message.OpCode}] {message.EventName}");
			};

			client.OnPacketReceived += async (message) =>
			{
				Console.WriteLine($"> [{message.OpCode}] {message.EventName}");
			};

			await client.StartAsync();
			await Task.Delay(-1);
		}
    }
}
