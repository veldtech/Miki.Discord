using Miki.Discord.Common;
using Miki.Discord.Common.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Miki.Discord.Internal
{
    public class DiscordGuildUser : DiscordUser, IDiscordGuildUser
    {
        private readonly DiscordGuildMemberPacket _packet;

        public DiscordGuildUser(DiscordGuildMemberPacket packet, IDiscordClient client)
            : base(packet.User, client)
        {
            _packet = packet;
        }

        public string Nickname
            => _packet.Nickname;

        public IReadOnlyCollection<ulong> RoleIds
            => _packet.Roles;

        public ulong GuildId
            => _packet.GuildId;

        public DateTimeOffset JoinedAt
            => new DateTimeOffset(_packet.JoinedAt, new TimeSpan(0));

        public DateTimeOffset PremiumSince
            => new DateTimeOffset(_packet.PremiumSince, new TimeSpan(0));

        public async Task AddRoleAsync(IDiscordRole role)
        {
            await Client.ApiClient.AddGuildMemberRoleAsync(GuildId, Id, role.Id);
        }

        public async Task<IDiscordGuild> GetGuildAsync()
            => await Client.GetGuildAsync(_packet.GuildId);

        public async Task KickAsync(string reason = null)
        {
            await Client.ApiClient.RemoveGuildMemberAsync(GuildId, Id, reason);
        }

        public async Task RemoveRoleAsync(IDiscordRole role)
        {
            if(role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }

            await Client.ApiClient.RemoveGuildMemberRoleAsync(GuildId, Id, role.Id);
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