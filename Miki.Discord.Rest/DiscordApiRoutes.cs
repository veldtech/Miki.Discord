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