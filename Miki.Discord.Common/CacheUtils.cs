using System;
using System.Collections.Generic;
using System.Text;

namespace Miki.Discord.Common
{
    public static class CacheUtils
    {
		/// <summary>
		/// Returns a DM channel cache key collection
		/// </summary>
		/// <returns></returns>
		public static string DirectChannelsKey()
			=> $"discord:dmchannels";

		/// <summary>
		/// Returns a user collection cache key
		/// </summary>
		/// <returns></returns>
		public static string UsersCacheKey()
			=> $"discord:users";

		public static string GuildsCacheKey()
			=> $"discord:guilds";

		public static string GuildChannelsKey(ulong guildId)
			=> $"{GuildsCacheKey()}:channels:{guildId}";

		public static string GuildMembersKey(ulong guildId)
			=> $"{GuildsCacheKey()}:members:{guildId}";

		public static string GuildRolesKey(ulong guildId)
			=> $"{GuildsCacheKey()}:roles:{guildId}";
	}
}
