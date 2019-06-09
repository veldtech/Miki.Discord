using Miki.Discord.Common.Events;
using Miki.Discord.Common.Gateway;
using Miki.Discord.Common.Gateway.Packets;
using Miki.Discord.Common.Packets;
using Miki.Discord.Common.Packets.Events;
using System;
using System.Threading.Tasks;

namespace Miki.Discord.Common
{
	public interface IGateway
	{
        #region Events
        Func<DiscordChannelPacket, Task> OnChannelCreate { get; set; }
		Func<DiscordChannelPacket, Task> OnChannelUpdate { get; set; }
		Func<DiscordChannelPacket, Task> OnChannelDelete { get; set; }

		Func<DiscordGuildPacket, Task> OnGuildCreate { get; set; }
		Func<DiscordGuildPacket, Task> OnGuildUpdate { get; set; }
		Func<DiscordGuildUnavailablePacket, Task> OnGuildDelete { get; set; }

		Func<DiscordGuildMemberPacket, Task> OnGuildMemberAdd { get; set; }
		Func<ulong, DiscordUserPacket, Task> OnGuildMemberRemove { get; set; }
		Func<GuildMemberUpdateEventArgs, Task> OnGuildMemberUpdate { get; set; }

		Func<ulong, DiscordUserPacket, Task> OnGuildBanAdd { get; set; }
		Func<ulong, DiscordUserPacket, Task> OnGuildBanRemove { get; set; }

		Func<ulong, DiscordEmoji[], Task> OnGuildEmojiUpdate { get; set; }

		Func<ulong, DiscordRolePacket, Task> OnGuildRoleCreate { get; set; }
		Func<ulong, DiscordRolePacket, Task> OnGuildRoleUpdate { get; set; }
		Func<ulong, ulong, Task> OnGuildRoleDelete { get; set; }

		Func<DiscordMessagePacket, Task> OnMessageCreate { get; set; }
		Func<DiscordMessagePacket, Task> OnMessageUpdate { get; set; }
		Func<MessageDeleteArgs, Task> OnMessageDelete { get; set; }
		Func<MessageBulkDeleteEventArgs, Task> OnMessageDeleteBulk { get; set; }

		Func<DiscordPresencePacket, Task> OnPresenceUpdate { get; set; }

		Func<GatewayReadyPacket, Task> OnReady { get; set; }

		Func<TypingStartEventArgs, Task> OnTypingStart { get; set; }

		Func<DiscordPresencePacket, Task> OnUserUpdate { get; set; }

        event Func<IGatewayMessage, Task> OnPacketSent;
        event Func<IGatewayMessage, Task> OnPacketReceived;
        #endregion

        Task RestartAsync();

        Task SendAsync(int shardId, GatewayOpcode opcode, object payload);

		Task StartAsync();

		Task StopAsync();
	}
}