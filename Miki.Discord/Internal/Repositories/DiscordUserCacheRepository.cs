using System.Threading.Tasks;
using Miki.Cache;
using Miki.Discord.Common;
using Miki.Discord.Common.Packets;

namespace Miki.Discord.Internal.Repositories
{
    public class DiscordUserCacheRepository : BaseCacheRepository<DiscordUserPacket>
    {
        private readonly IExtendedCacheClient cacheClient;
        private readonly IApiClient apiClient;

        public DiscordUserCacheRepository(IExtendedCacheClient cacheClient, IApiClient apiClient)    
            : base(cacheClient)
        {
            this.cacheClient = cacheClient;
            this.apiClient = apiClient;
        }

        protected override string GetCacheKey(DiscordUserPacket value)
        {
            return CacheHelpers.UsersCacheKey;
        }

        protected override string GetMemberKey(DiscordUserPacket value)
        {
            return value.Id.ToString();
        }

        protected override async ValueTask<DiscordUserPacket> GetFromCacheAsync(params object[] id)
        {
            return await cacheClient.HashGetAsync<DiscordUserPacket>(CacheHelpers.UsersCacheKey, id[0].ToString());
        }

        protected override async ValueTask<DiscordUserPacket> GetFromApiAsync(params object[] id)
        {
            return await apiClient.GetUserAsync((ulong) id[0]);
        }
    }
}