using Miki.Discord.Common;
using Miki.Discord.Common.Events;
using Miki.Discord.Common.Gateway;
using Miki.Discord.Common.Gateway.Packets;
using Miki.Discord.Common.Packets;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Miki.Discord.Tests.Dummy
{
	class DummyGateway : IGateway
	{
		public Func<DiscordChannelPacket, Task> OnChannelCreate { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public Func<DiscordChannelPacket, Task> OnChannelUpdate { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public Func<DiscordChannelPacket, Task> OnChannelDelete { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public Func<DiscordGuildPacket, Task> OnGuildCreate { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public Func<DiscordGuildPacket, Task> OnGuildUpdate { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public Func<DiscordGuildUnavailablePacket, Task> OnGuildDelete { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public Func<DiscordGuildMemberPacket, Task> OnGuildMemberAdd { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public Func<ulong, DiscordUserPacket, Task> OnGuildMemberRemove { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public Func<GuildMemberUpdateEventArgs, Task> OnGuildMemberUpdate { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public Func<ulong, DiscordUserPacket, Task> OnGuildBanAdd { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public Func<ulong, DiscordUserPacket, Task> OnGuildBanRemove { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public Func<ulong, DiscordRolePacket, Task> OnGuildRoleCreate { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public Func<ulong, DiscordRolePacket, Task> OnGuildRoleUpdate { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public Func<ulong, ulong, Task> OnGuildRoleDelete { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public Func<DiscordMessagePacket, Task> OnMessageCreate { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public Func<DiscordMessagePacket, Task> OnMessageUpdate { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public Func<MessageDeleteArgs, Task> OnMessageDelete { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public Func<DiscordMessagePacket, Task> OnMessageDeleteBulk { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public Func<DiscordPresencePacket, Task> OnPresenceUpdate { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public Func<Task> OnReady { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public Func<DiscordUserPacket, Task> OnUserUpdate { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public Func<GatewayMessage, Task> OnPacketSent { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public Func<GatewayMessage, Task> OnPacketReceived { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

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
