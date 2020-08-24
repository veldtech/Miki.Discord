using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Miki.Cache;
using Miki.Discord.Common;
using Miki.Discord.Common.Packets;
using Miki.Discord.Internal.Repositories;
using Miki.Patterns.Repositories;

namespace Miki.Discord.Cache
{
    /// <summary>
    /// Handles entity caching
    /// </summary>
    public class DefaultCacheHandler : ICacheHandler
    {
        private readonly IExtendedCacheClient cache;

        /// <inheritdoc />
        public IAsyncRepository<DiscordChannelPacket> Channels { get; }

        /// <inheritdoc />
        public IAsyncRepository<DiscordGuildPacket> Guilds { get; }

        /// <inheritdoc />
        public IAsyncRepository<DiscordGuildMemberPacket> Members { get; }
        
        /// <inheritdoc />
        public IAsyncRepository<DiscordRolePacket> Roles { get; }

        /// <inheritdoc />
        public IAsyncRepository<DiscordUserPacket> Users { get; }

        /// <summary>
        /// Default caching strategy for Miki.Discord
        /// </summary>
        /// <param name="cache">Cache provider</param>
        public DefaultCacheHandler(IExtendedCacheClient cache, IApiClient apiClient)
        {
            this.cache = cache;
            
            Channels = new DiscordChannelCacheRepository(cache, apiClient);
            Guilds = new DiscordGuildCacheRepository(cache, apiClient);
            Members = new DiscordMemberCacheRepository(cache, apiClient);
            Roles = new DiscordRoleCacheRepository(cache, apiClient);
            Users = new DiscordUserCacheRepository(cache, apiClient);
        }

        /// <inhericdoc />
        public async ValueTask<DiscordUserPacket> GetCurrentUserAsync()
        {
            return await cache.HashGetAsync<DiscordUserPacket>(CacheHelpers.UsersCacheKey, "me");
        }

        public async ValueTask<IReadOnlyList<DiscordChannelPacket>> GetChannelsFromGuildAsync(ulong guildId)
        {
            return (await cache.HashValuesAsync<DiscordChannelPacket>(CacheHelpers.ChannelsKey(guildId))).ToList();
        }

        public async ValueTask<IReadOnlyList<DiscordGuildMemberPacket>> GetMembersFromGuildAsync(ulong guildId)
        {
            return (await cache.HashValuesAsync<DiscordGuildMemberPacket>(CacheHelpers.GuildMembersKey(guildId))).ToList();
        }

        public async ValueTask<IReadOnlyList<DiscordRolePacket>> GetRolesFromGuildAsync(ulong guildId)
        {
            return (await cache.HashValuesAsync<DiscordRolePacket>(CacheHelpers.GuildRolesKey(guildId))).ToList();
        }

        /// <inheritdoc />
        public async ValueTask SetCurrentUserAsync(DiscordUserPacket packet)
        {
            await cache.HashUpsertAsync(CacheHelpers.UsersCacheKey, "me", packet);
        }
        public async ValueTask<bool> HasGuildAsync(ulong guildId)
        {
            var guild = await Guilds.GetAsync(guildId);
            return guild != null;
        }
    }
}