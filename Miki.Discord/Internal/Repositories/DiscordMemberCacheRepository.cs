using System.Threading.Tasks;
using Miki.Cache;
using Miki.Discord.Common;

namespace Miki.Discord.Internal.Repositories
{
    internal class DiscordMemberCacheRepository : BaseCacheRepository<DiscordGuildMemberPacket>
    {
        private readonly IExtendedCacheClient cacheClient;
        private readonly IApiClient apiClient;
        
        public DiscordMemberCacheRepository(IExtendedCacheClient cacheClient, IApiClient apiClient)
            : base(cacheClient)
        {
            this.cacheClient = cacheClient;
            this.apiClient = apiClient;
        }

        protected override string GetCacheKey(DiscordGuildMemberPacket value)
            => CacheHelpers.GuildMembersKey(value.GuildId);
        
        protected override string GetMemberKey(DiscordGuildMemberPacket value)
            => value.User.Id.ToString();

        protected override async ValueTask<DiscordGuildMemberPacket> GetFromCacheAsync(params object[] id)
        {
            return await cacheClient.HashGetAsync<DiscordGuildMemberPacket>(
                CacheHelpers.GuildMembersKey((ulong)id[1]), id[0].ToString());
        }

        protected override async ValueTask<DiscordGuildMemberPacket> GetFromApiAsync(params object[] id)
        {
            return await apiClient.GetGuildUserAsync((ulong) id[0], (ulong) id[1]);
        }
    }
}