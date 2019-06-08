using Miki.Discord.Common;
using System.Runtime.CompilerServices;

namespace Miki.Discord.Rest
{
	internal static class DiscordApiRoutes
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static string ChannelMessage(
            ulong channelId, 
            ulong messageId)
			=> $"{ChannelMessages(channelId)}/{messageId}";

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static string ChannelTyping(
            ulong channelId)
			=> $"{Channel(channelId)}/typing";

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static string ChannelBulkDeleteMessages(
            ulong channelId)
			=> $"{Channel(channelId)}/messages/bulk-delete";

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static string ChannelMessages(
            ulong channelId)
			=> $"{Channel(channelId)}/messages";

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static string Channel(
            ulong channelId)
			=> $"/channels/{channelId}";

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static string GuildBan(
            ulong guildId, 
            ulong userId)
			=> $"{Guild(guildId)}/bans/{userId}";

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static string GuildChannels(
            ulong guildId)
			=> $"{Guild(guildId)}/channels";

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static string GuildEmoji(
            ulong guildId)
			=> $"{Guild(guildId)}/emojis";

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static string GuildEmoji(
            ulong guildId, 
            ulong emojiId)
			=> $"{GuildEmoji(guildId)}/{emojiId}";

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static string GuildMember(
            ulong guildId, 
            ulong userId)
			=> $"{Guild(guildId)}/members/{userId}";

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static string GuildMemberRole(
            ulong guildId, 
            ulong userId, 
            ulong roleId)
			=> $"{GuildMember(guildId, userId)}/roles/{roleId}";

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string GuildPrune(
            ulong guildId)
            => $"{Guild(guildId)}/prune";

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static string GuildRoles(
            ulong guildId)
			=> $"{Guild(guildId)}/roles";

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string GuildRole(
            ulong guildId, 
            ulong roleId)
			=> $"{GuildRoles(guildId)}/{roleId}";

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static string Guild(
            ulong guildId)
			=> $"/guilds/{guildId}";

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static string MessageReactions(
            ulong channelId, 
            ulong messageId)
			=> $"{ChannelMessage(channelId, messageId)}/reactions";

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static string MessageReaction(
            ulong channelId, 
            ulong messageId, 
            DiscordEmoji emoji)
			=> $"{MessageReactions(channelId, messageId)}/{emoji.ToString()}";

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static string MessageReaction(
            ulong channelId, 
            ulong messageId, 
            DiscordEmoji emoji, 
            ulong userId)
			=> $"{MessageReaction(channelId, messageId, emoji)}/{userId}";

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static string MessageReactionMe(
            ulong channelId, 
            ulong messageId, 
            DiscordEmoji emoji)
			=> $"{MessageReaction(channelId, messageId, emoji)}/@me";

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static string UserMe()
			=> $"/users/@me";

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static string UserMeChannels()
			=> $"{UserMe()}/channels";

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static string User(
            ulong userId)
			=> $"/users/{userId}";

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static string Gateway()
			=> $"/gateway";

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static string BotGateway()
			=> $"/gateway/bot";
	}
}