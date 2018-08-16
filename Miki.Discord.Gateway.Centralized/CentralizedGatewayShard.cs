using Miki.Discord.Common;
using Miki.Discord.Common.Events;
using Miki.Discord.Common.Gateway;
using Miki.Discord.Common.Gateway.Packets;
using Miki.Discord.Common.Packets;
using Miki.Discord.Rest.Entities;
using Miki.Net.WebSockets;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Miki.Discord.Gateway.Centralized
{
    public class CentralizedGatewayShard : IGateway
	{
		private GatewayConfiguration _configuration;
		private GatewayConnection _connection;

		public CentralizedGatewayShard(GatewayConfiguration configuration)
		{
			_configuration = configuration;
			_connection = new GatewayConnection(configuration);
		}

		public async Task StartAsync()
		{
			_connection.OnTextPacket += OnTextPacketReceivedAsync;

			await _connection.StartAsync();
		}

		public async Task StopAsync()
		{
			_connection.OnTextPacket -= OnTextPacketReceivedAsync;

			await _connection.StopAsync();
		}

		public async Task OnTextPacketReceivedAsync(string text)
		{
			Console.WriteLine(text);

			GatewayMessage msg = JsonConvert.DeserializeObject<GatewayMessage>(text);

			switch (msg.OpCode)
			{
				case GatewayOpcode.Dispatch:
				{

				} break;
				case GatewayOpcode.Heartbeat:
				{

				} break;
				case GatewayOpcode.Reconnect:
				{
					
				} break;
				case GatewayOpcode.InvalidSession:
				{
					throw new Exception("oh no.");
				}
				case GatewayOpcode.Hello:
				{
					await IdentifyAsync(msg.Data.ToObject<GatewayHelloPacket>());
				} break;
				case GatewayOpcode.HeartbeatAcknowledge:
				{

				} break;
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
			identifyPacket.Presence = null;
			identifyPacket.LargeThreshold = 250;

			string serializedPacket = JsonConvert.SerializeObject(identifyPacket);
			CancellationTokenSource token = new CancellationTokenSource();

			await _connection.WebSocketClient.SendAsync(serializedPacket, token.Token);
		}

		public Task SendAsync(int shardId, GatewayOpcode opcode, object payload)
		{
			throw new NotImplementedException();
		}

		#region Events		
		public Func<DiscordChannelPacket, Task> OnChannelCreate { get; set; }
		public Func<DiscordChannelPacket, Task> OnChannelUpdate { get; set; }
		public Func<DiscordChannelPacket, Task> OnChannelDelete { get; set; }

		public Func<DiscordGuildPacket, Task> OnGuildCreate { get; set; }
		public Func<DiscordGuildPacket, Task> OnGuildUpdate { get; set; }
		public Func<DiscordGuildUnavailablePacket, Task> OnGuildDelete { get; set; }

		public Func<DiscordGuildMemberPacket, Task> OnGuildMemberAdd { get; set; }
		public Func<ulong, DiscordUserPacket, Task> OnGuildMemberRemove { get; set; }
		public Func<GuildMemberUpdateEventArgs, Task> OnGuildMemberUpdate { get; set; }

		public Func<ulong, DiscordUserPacket, Task> OnGuildBanAdd { get; set; }
		public Func<ulong, DiscordUserPacket, Task> OnGuildBanRemove { get; set; }

		public Func<ulong, DiscordRolePacket, Task> OnGuildRoleCreate { get; set; }
		public Func<ulong, DiscordRolePacket, Task> OnGuildRoleUpdate { get; set; }
		public Func<ulong, ulong, Task> OnGuildRoleDelete { get; set; }

		public Func<DiscordMessagePacket, Task> OnMessageCreate { get; set; }
		public Func<DiscordMessagePacket, Task> OnMessageUpdate { get; set; }
		public Func<MessageDeleteArgs, Task> OnMessageDelete { get; set; }
		public Func<DiscordMessagePacket, Task> OnMessageDeleteBulk { get; set; }

		public Func<DiscordPresencePacket, Task> OnPresenceUpdate { get; set; }

		public Func<Task> OnReady { get; set; }

		public Func<DiscordUserPacket, Task> OnUserUpdate { get; set; }

		public Func<GatewayMessage, Task> OnPacketSent { get; set; }
		public Func<GatewayMessage, Task> OnPacketReceived { get; set; }
		#endregion
	}
}
