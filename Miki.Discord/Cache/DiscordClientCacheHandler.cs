using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Miki.Cache;
using Miki.Discord.Common;
using Miki.Discord.Common.Packets;

namespace Miki.Discord.Cache
{
    public class DefaultCacheHandler : ICacheHandler
    {
        private readonly IExtendedCacheClient cache;

        public DefaultCacheHandler(IExtendedCacheClient cache)
        {
            this.cache = cache;
        }

        /// <inhericdoc />
        public async ValueTask<DiscordUserPacket> GetCurrentUserAsync()
        {
            return await cache.HashGetAsync<DiscordUserPacket>(CacheUtils.UsersCacheKey, "me");
        }

        /// <inheritdoc />
        public async ValueTask<DiscordGuildPacket> GetGuildAsync(ulong guildId)
        {
            return await cache.HashGetAsync<DiscordGuildPacket>(
                CacheUtils.GuildsCacheKey, guildId.ToString());
        }

        /// <inheritdoc />
        public async ValueTask<DiscordGuildMemberPacket> GetGuildMemberAsync(
            ulong guildId, ulong userId)
        {
            return await cache.HashGetAsync<DiscordGuildMemberPacket>(
                CacheUtils.GuildMembersKey(guildId), userId.ToString());
        }

        /// <inheritdoc />
        public async ValueTask<DiscordRolePacket> GetGuildRoleAsync(ulong guildId, ulong roleId)
        {
            return await cache.HashGetAsync<DiscordRolePacket>(
                CacheUtils.GuildRolesKey(guildId), roleId.ToString());
        }

        /// <inheritdoc />
        public async ValueTask<IReadOnlyList<DiscordRolePacket>> GetGuildRolesAsync(ulong guildId)
        {
            var roles = await cache.HashValuesAsync<DiscordRolePacket>(
                CacheUtils.GuildRolesKey(guildId));
            return roles?.ToList();
        }

        public async ValueTask<DiscordUserPacket> GetUserAsync(ulong userId)
        {
            return await cache.HashGetAsync<DiscordUserPacket>(
                CacheUtils.UsersCacheKey, userId.ToString());
        }
        
        /// <inheritdoc />
        public async ValueTask SetCurrentUserAsync(DiscordUserPacket packet)
        {
            await cache.HashUpsertAsync(CacheUtils.UsersCacheKey, "me", packet);
        }

        /// <inheritdoc />
        public async ValueTask SetGuildAsync(DiscordGuildPacket packet)
        {
            await cache.HashUpsertAsync(CacheUtils.GuildsCacheKey, packet.Id.ToString(), packet);
        }

        /// <inheritdoc />
        public async ValueTask SetGuildMemberAsync(DiscordGuildMemberPacket packet)
        {
            await cache.HashUpsertAsync(
                CacheUtils.GuildMembersKey(packet.GuildId), packet.User.Id.ToString(), packet);
        }

        /// <inheritdoc />
        public async ValueTask SetGuildRoleAsync(ulong guildId, DiscordRolePacket role)
        {
            await cache.HashUpsertAsync(
                CacheUtils.GuildRolesKey(guildId), role.Id.ToString(), role);
        }

        /// <inheritdoc />
        public async ValueTask SetGuildRolesAsync(ulong guildId, IEnumerable<DiscordRolePacket> roles)
        {
            await cache.HashUpsertAsync(
                CacheUtils.GuildRolesKey(guildId), 
                roles.Select(x => new KeyValuePair<string, DiscordRolePacket>(x.Id.ToString(), x)));
        }

        public async ValueTask SetUserAsync(DiscordUserPacket packet)
        {
            await cache.HashUpsertAsync(CacheUtils.UsersCacheKey, packet.Id.ToString(), packet);
        }
    }

    public interface ICacheHandler
    {
        ValueTask<DiscordUserPacket> GetCurrentUserAsync();
        ValueTask<DiscordGuildPacket> GetGuildAsync(ulong guildId);
        ValueTask<DiscordGuildMemberPacket> GetGuildMemberAsync(ulong guildId, ulong userId);
        ValueTask<DiscordRolePacket> GetGuildRoleAsync(ulong guildId, ulong roleId);
        ValueTask<IReadOnlyList<DiscordRolePacket>> GetGuildRolesAsync(ulong guildId);
        ValueTask<DiscordUserPacket> GetUserAsync(ulong userId);
        ValueTask SetCurrentUserAsync(DiscordUserPacket user);
        ValueTask SetGuildAsync(DiscordGuildPacket packet);
        ValueTask SetGuildMemberAsync(DiscordGuildMemberPacket packet);
        ValueTask SetGuildRoleAsync(ulong guildId, DiscordRolePacket role);
        ValueTask SetGuildRolesAsync(ulong guildId, IEnumerable<DiscordRolePacket> roles);
        ValueTask SetUserAsync(DiscordUserPacket packet);
    }
}
