namespace Miki.Discord.Common
{
    public static class CacheUtils
    {
        /// <summary>
        /// Returns a DM channel cache key collection
        /// </summary>
        /// <returns></returns>
        public static string ChannelsKey(ulong? guildId = null)
        {
            if(guildId.HasValue)
            {
                return $"{GuildsCacheKey}:channels:{guildId}";
            }
            else
            {
                return $"discord:dmchannels";
            }
        }

        /// <summary>
        /// Returns a user collection cache key
        /// </summary>
        /// <returns></returns>
        public const string UsersCacheKey = "discord:users";

        public const string GuildsCacheKey = "discord:guilds";

        public static string GuildMembersKey(ulong guildId)
            => $"{GuildsCacheKey}:members:{guildId}";

        public static string GuildRolesKey(ulong guildId)
            => $"{GuildsCacheKey}:roles:{guildId}";

        public static string GuildPresencesKey()
            => $"{UsersCacheKey}:presences";

        public const string EmojiCacheKey = "discord:emoji";
    }
}