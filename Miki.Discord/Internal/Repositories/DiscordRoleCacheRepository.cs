using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Miki.Cache;
using Miki.Discord.Common;
using Miki.Patterns.Repositories;

namespace Miki.Discord.Internal.Repositories
{
    internal class DiscordRoleCacheRepository : BaseCacheRepository<DiscordRolePacket>
    {
        private readonly IExtendedCacheClient cacheClient;
        private readonly IApiClient apiClient;

        public DiscordRoleCacheRepository(IExtendedCacheClient cacheClient, IApiClient apiClient)
            : base(cacheClient)
        {
            this.cacheClient = cacheClient;
            this.apiClient = apiClient;
        }

        protected override string GetCacheKey(DiscordRolePacket value)
        {
            return CacheHelpers.GuildRolesKey(value.GuildId);
        }

        protected override string GetMemberKey(DiscordRolePacket value)
        {
            return value.Id.ToString();
        }

        protected override async ValueTask<DiscordRolePacket> GetFromCacheAsync(params object[] id)
        {
            var rolePacket = await cacheClient.HashGetAsync<DiscordRolePacket>(
                CacheHelpers.GuildRolesKey((ulong) id[1]), id[0].ToString());

            if (rolePacket != null)
            {
                rolePacket.GuildId = (ulong) id[1];
            }

            return rolePacket;
        }

        protected override async ValueTask<DiscordRolePacket> GetFromApiAsync(params object[] id)
        {
            var roles = await apiClient.GetRolesAsync((ulong) id[1]);

            var role = roles?.FirstOrDefault(x => x.Id == (ulong) id[0]);

            if (role != null)
            {
                role.GuildId = (ulong) id[1];
            }

            return role;
        }
    }
}