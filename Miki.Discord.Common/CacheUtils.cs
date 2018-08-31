using System;
using System.Collections.Generic;
using System.Text;

namespace Miki.Discord.Common
{
    public static class CacheUtils
    {
		public static string DirectChannels()
			=> $"discord:dmchannels";

		public static string GuildsCacheKey(ulong guildId)
			=> $"discord:guild:{guildId}";

		public static string GuildChannelsKey(ulong guildId)
			=> $"dicord:channels:{guildId}";

		public static string GuildMembersKey(ulong guildId)
			=> $"discord:members:{guildId}";
	}
}
