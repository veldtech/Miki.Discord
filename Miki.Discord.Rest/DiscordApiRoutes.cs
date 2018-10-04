using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Miki.Discord.Rest
{
    internal static class DiscordApiRoutes
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static string ChannelMessageRoute(ulong channelId, ulong messageId)
			=> $"{ChannelMessagesRoute(channelId)}/{messageId}";

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static string ChannelTypingRoute(ulong channelId)
			=> $"{ChannelRoute(channelId)}/typing";

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static string ChannelBulkDeleteMessages(ulong channelId)
			=> $"{ChannelRoute(channelId)}/messages/bulk-delete";

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static string ChannelMessagesRoute(ulong channelId)
			=> $"{ChannelRoute(channelId)}/messages";

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static string ChannelRoute(ulong channelId)
			=> $"/channels/{channelId}";

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static string GuildBanRoute(ulong guildId, ulong userId)
			=> $"{GuildRoute(guildId)}/bans/{userId}";

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static string GuildChannelsRoute(ulong guildId)
			=> $"{GuildRoute(guildId)}/channels";

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static string GuildEmojiRoute(ulong guildId)
			=> $"{GuildRoute(guildId)}/emojis";

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static string GuildEmojiRoute(ulong guildId, ulong emojiId)
			=> $"{GuildEmojiRoute(guildId)}/{emojiId}";

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static string GuildMemberRoute(ulong guildId, ulong userId)
			=> $"{GuildRoute(guildId)}/members/{userId}";

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static string GuildMemberRoleRoute(ulong guildId, ulong userId, ulong roleId)
			=> $"{GuildMemberRoute(guildId, userId)}/roles/{roleId}";

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static string GuildRolesRoute(ulong guildId)
			=> $"{GuildRoute(guildId)}/roles";

		internal static string GuildRoleRoute(ulong guildId, ulong roleId)
			=> $"{GuildRolesRoute(guildId)}/{roleId}";

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static string GuildRoute(ulong guildId)
			=> $"/guilds/{guildId}";

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static string MessageReactionsRoute(ulong channelId, ulong messageId)
			=> $"{ChannelMessageRoute(channelId, messageId)}/reactions";

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static string MessageReactionRoute(ulong channelId, ulong messageId, ulong emojiId)
			=> $"{MessageReactionsRoute(channelId, messageId)}/{emojiId}";

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static string MessageReactionRoute(ulong channelId, ulong messageId, ulong emojiId, ulong userId)
			=> $"{MessageReactionRoute(channelId, messageId, emojiId)}/{userId}";

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static string MessageReactionMeRoute(ulong channelId, ulong messageId, ulong emojiId)
			=> $"{MessageReactionRoute(channelId, messageId, emojiId)}/@me";

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static string UserMeRoute()
			=> $"/users/@me";

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static string UserMeChannelsRoute()
			=> $"{UserMeRoute()}/channels";

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static string UserRoute(ulong userId)
			=> $"/users/{userId}";

	}
}