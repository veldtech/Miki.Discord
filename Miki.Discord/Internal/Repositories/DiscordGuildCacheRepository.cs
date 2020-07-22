using System.Threading.Tasks;
using Miki.Cache;
using Miki.Discord.Common;

namespace Miki.Discord.Internal.Repositories
{
    public class DiscordGuildCacheRepository : BaseCacheRepository<DiscordGuildPacket>
    {
        private readonly IExtendedCacheClient cacheClient;
        private readonly IApiClient apiClient;

        public DiscordGuildCacheRepository(IExtendedCacheClient cacheClient, IApiClient apiClient)    
            : base(cacheClient)
        {
            this.cacheClient = cacheClient;
            this.apiClient = apiClient;
        }

        protected override string GetCacheKey(DiscordGuildPacket value)
        {
            return CacheHelpers.GuildsCacheKey;
        }

        protected override string GetMemberKey(DiscordGuildPacket value)
        {
            return value.Id.ToString();
        }

        protected override async ValueTask<DiscordGuildPacket> GetFromCacheAsync(params object[] id)
        {
            return await cacheClient.HashGetAsync<DiscordGuildPacket>(CacheHelpers.GuildsCacheKey, id[0].ToString());
        }

        protected override async ValueTask<DiscordGuildPacket> GetFromApiAsync(params object[] id)
        {
            return await apiClient.GetGuildAsync((ulong) id[0]);
        }
    }
}