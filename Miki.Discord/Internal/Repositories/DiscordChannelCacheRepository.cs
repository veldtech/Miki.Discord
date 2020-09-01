using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Miki.Cache;
using Miki.Discord.Common;
using Miki.Patterns.Repositories;

namespace Miki.Discord.Internal.Repositories
{
    internal class DiscordChannelCacheRepository : BaseCacheRepository<DiscordChannelPacket>
    {
        private readonly IExtendedCacheClient cacheClient;
        private readonly IApiClient apiClient;

        public DiscordChannelCacheRepository(IExtendedCacheClient cacheClient, IApiClient apiClient)
            : base(cacheClient)
        {
            this.cacheClient = cacheClient;
            this.apiClient = apiClient;
        }

        protected override string GetCacheKey(DiscordChannelPacket value)
        {
            return CacheHelpers.ChannelsKey(value.GuildId);
        }

        protected override string GetMemberKey(DiscordChannelPacket value)
        {
            return value.Id.ToString();
        }

        protected override async ValueTask<DiscordChannelPacket> GetFromCacheAsync(params object[] id)
        {
            if (id.Length == 1 || id[1] == null)
            {
                return await cacheClient.HashGetAsync<DiscordChannelPacket>(
                    CacheHelpers.ChannelsKey(), id[0].ToString());
            }

            return await cacheClient.HashGetAsync<DiscordChannelPacket>(
                CacheHelpers.ChannelsKey((ulong) id[1]), id[0].ToString());
        }

        protected override async ValueTask<DiscordChannelPacket> GetFromApiAsync(params object[] id)
        {
            return await apiClient.GetChannelAsync((ulong) id[0]);
        }
    }
}