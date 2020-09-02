using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Miki.Discord.Common;

namespace Miki.Discord.Internal.Data
{
    public class DiscordGuildUser : DiscordUser, IDiscordGuildUser
    {
        private readonly DiscordGuildMemberPacket packet;

        public DiscordGuildUser(DiscordGuildMemberPacket packet, IDiscordClient client)
            : base(packet.User, client)
        {
            this.packet = packet;
        }

        public string Nickname
            => packet.Nickname;

        public IReadOnlyCollection<ulong> RoleIds
            => packet.Roles;

        public ulong GuildId
            => packet.GuildId;

        public DateTimeOffset JoinedAt
            => new DateTimeOffset(packet.JoinedAt);

        public DateTimeOffset? PremiumSince
            => packet.PremiumSince.HasValue 
                ? new DateTimeOffset(packet.PremiumSince.Value) 
                : (DateTimeOffset?) null;

        public async Task AddRoleAsync(IDiscordRole role)
        {
            await client.ApiClient.AddGuildMemberRoleAsync(GuildId, Id, role.Id);
        }

        public async Task<IDiscordGuild> GetGuildAsync()
            => await client.GetGuildAsync(GuildId);

        public async Task KickAsync(string reason = null)
        {
            await client.ApiClient.RemoveGuildMemberAsync(GuildId, Id, reason);
        }

        public async Task RemoveRoleAsync(IDiscordRole role)
        {
            if(role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }

            await client.ApiClient.RemoveGuildMemberRoleAsync(GuildId, Id, role.Id);
        }

        public async Task<IEnumerable<IDiscordRole>> GetRolesAsync()
        {
            var guild = await GetGuildAsync();
            var roles = await guild.GetRolesAsync();
            return roles.Where(x => RoleIds.Contains(x.Id));
        }

        public async Task<bool> HasPermissionsAsync(GuildPermission permissions)
        {
            var guild = await GetGuildAsync();
            GuildPermission p = await guild.GetPermissionsAsync(this);
            return p.HasFlag(permissions);
        }

        public async Task<int> GetHierarchyAsync()
        {
            var guild = await GetGuildAsync();
            return (await guild.GetRolesAsync())
                .Where(x => RoleIds.Contains(x.Id))
                .Max(x => x.Position);
        }
    }
}