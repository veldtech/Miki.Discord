using Miki.Discord.Common;
using Miki.Discord.Common.Events;
using Miki.Discord.Common.Gateway;
using Miki.Discord.Common.Gateway.Packets;
using Miki.Discord.Common.Packets;
using Miki.Discord.Common.Packets.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Miki.Discord.Tests.Dummy
{
	public class DummyGateway : IGateway
	{
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
		public Func<ulong, DiscordEmoji[], Task> OnGuildEmojiUpdate { get; set; }
		public Func<ulong, DiscordRolePacket, Task> OnGuildRoleCreate { get; set; }
		public Func<ulong, DiscordRolePacket, Task> OnGuildRoleUpdate { get; set; }
		public Func<ulong, ulong, Task> OnGuildRoleDelete { get; set; }
		public Func<DiscordMessagePacket, Task> OnMessageCreate { get; set; }
		public Func<DiscordMessagePacket, Task> OnMessageUpdate { get; set; }
		public Func<MessageDeleteArgs, Task> OnMessageDelete { get; set; }
		public Func<DiscordPresencePacket, Task> OnPresenceUpdate { get; set; }
		public Func<GatewayReadyPacket, Task> OnReady { get; set; }
		public Func<DiscordPresencePacket, Task> OnUserUpdate { get; set; }
		public Func<GatewayMessage, Task> OnPacketSent { get; set; }
		public Func<GatewayMessage, Task> OnPacketReceived { get; set; }
		public Func<TypingStartEventArgs, Task> OnTypingStart { get; set; }
		public Func<MessageBulkDeleteEventArgs, Task> OnMessageDeleteBulk { get; set; }

		public Task SendAsync(int shardId, GatewayOpcode opcode, object payload)
		{
			throw new NotImplementedException();
		}

		public Task StartAsync()
		{
			throw new NotImplementedException();
		}

		public Task StopAsync()
		{
			throw new NotImplementedException();
		}
	}
}