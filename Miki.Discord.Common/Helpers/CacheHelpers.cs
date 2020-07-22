namespace Miki.Discord.Common
{
    /// <summary>
    /// Helper class for cache related operations in Miki.Discord.
    /// </summary>
    public static class CacheHelpers
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

            return $"discord:dmchannels";
        }

        /// <summary>
        /// Returns a user collection cache key
        /// </summary>
        public const string UsersCacheKey = "discord:users";

        /// <summary>
        /// Guilds collection cache key.
        /// </summary>
        public const string GuildsCacheKey = "discord:guilds";

        /// <summary>
        /// Guild members cache key, indexes all members per guild.
        /// </summary>
        public static string GuildMembersKey(ulong guildId)
            => $"{GuildsCacheKey}:members:{guildId}";

        /// <summary>
        /// Guild roles cache key, indexes all roles per guild.
        /// </summary>
        public static string GuildRolesKey(ulong guildId)
            => $"{GuildsCacheKey}:roles:{guildId}";

        /// <summary>
        /// Guild presences cache key, indexes all presences per guild.
        /// </summary>
        public static string GuildPresencesKey()
            => $"{UsersCacheKey}:presences";
    }
}