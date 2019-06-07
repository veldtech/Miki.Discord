using Miki.Discord.Common.Events;
using Miki.Discord.Common.Gateway;
using Miki.Discord.Common.Packets;
using Miki.Discord.Common.Packets.Arguments;
using Miki.Discord.Rest;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Miki.Discord.Common
{
	public interface IApiClient : IGatewayApiClient
	{
		Task AddGuildBanAsync(ulong guildId, ulong userId, int pruneDays = 7, string reason = null);

		Task AddGuildMemberRoleAsync(ulong guildId, ulong userId, ulong roleId);

		Task<DiscordChannelPacket> CreateDMChannelAsync(ulong userId);

		Task<DiscordEmoji> CreateEmojiAsync(ulong guildId, EmojiCreationArgs args);

		Task CreateReactionAsync(ulong channelId, ulong messageId, DiscordEmoji emoji);

		Task<DiscordRolePacket> CreateGuildRoleAsync(ulong guildId, CreateRoleArgs args);

		Task DeleteChannelAsync(ulong channelId);

		Task DeleteGuildAsync(ulong guildId);

		Task DeleteReactionAsync(ulong channelId, ulong messageId, DiscordEmoji emoji);

		Task DeleteReactionAsync(ulong channelId, ulong messageId, DiscordEmoji emoji, ulong userId);

		Task DeleteReactionsAsync(ulong channelId, ulong messageId);

		Task DeleteMessageAsync(ulong channelId, ulong messageId);

		Task DeleteMessagesAsync(ulong channelId, ulong[] messages);

		Task<DiscordMessagePacket> EditMessageAsync(ulong channelId, ulong messageId, EditMessageArgs args);

		Task<DiscordRolePacket> EditRoleAsync(ulong guildId, DiscordRolePacket role);

		Task<DiscordUserPacket> GetCurrentUserAsync();

		Task<DiscordChannelPacket> GetChannelAsync(ulong channelId);

		Task<IReadOnlyList<DiscordChannelPacket>> GetChannelsAsync(ulong guildId);

        Task<IReadOnlyList<DiscordChannelPacket>> GetDMChannelsAsync();

        Task<DiscordGuildPacket> GetGuildAsync(ulong guildId);

		Task<DiscordGuildMemberPacket> GetGuildUserAsync(ulong userId, ulong guildId);

		Task<DiscordMessagePacket> GetMessageAsync(ulong channelId, ulong messageId);

		Task<IReadOnlyList<DiscordMessagePacket>> GetMessagesAsync(ulong channelId, int amount = 100);

		Task<DiscordUserPacket[]> GetReactionsAsync(ulong channelId, ulong messageId, DiscordEmoji emojiId);

		Task<DiscordRolePacket> GetRoleAsync(ulong roleId, ulong guildId);

		Task<IReadOnlyList<DiscordRolePacket>> GetRolesAsync(ulong guildId);

		Task<DiscordUserPacket> GetUserAsync(ulong userId);

		Task ModifyGuildMemberAsync(ulong guildId, ulong userId, ModifyGuildMemberArgs packet);

        Task ModifySelfAsync(UserModifyArgs args);

		Task RemoveGuildBanAsync(ulong guildId, ulong userId);

		Task RemoveGuildMemberAsync(ulong guildId, ulong userId, string reason = null);

		Task RemoveGuildMemberRoleAsync(ulong guildId, ulong userId, ulong roleId);

		Task<DiscordMessagePacket> SendFileAsync(ulong channelId, Stream stream, string fileName, MessageArgs args);

		Task<DiscordMessagePacket> SendMessageAsync(ulong channelId, MessageArgs args);

		Task TriggerTypingAsync(ulong channelId);
	}
}