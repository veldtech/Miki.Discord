using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Miki.Discord.Common;

namespace Miki.Discord.Internal.Data
{
    public class DiscordGuild : IDiscordGuild
    {
        private readonly DiscordGuildPacket packet;
        private readonly IDiscordClient client;

        public DiscordGuild(DiscordGuildPacket packet, IDiscordClient client)
        {
            this.packet = packet;
            this.client = client;
        }

        /// <inheritdoc />
        public string Name
            => packet.Name;

        /// <inheritdoc />
        public ulong Id
            => packet.Id;

        /// <inheritdoc />
        public string IconUrl
            => packet.Icon == null 
                ? DiscordHelpers.GetAvatarUrl(0)
                : DiscordHelpers.GetIconUrl(packet);

        /// <inheritdoc />
        public ulong OwnerId
            => packet.OwnerId;

        /// <inheritdoc />
        public int MemberCount
            => packet.MemberCount ?? 0;

        /// <inheritdoc />
        public GuildPermission Permissions
            => (GuildPermission)packet.Permissions.GetValueOrDefault(0);
        
        /// <inheritdoc />
        public int PremiumSubscriberCount
            => packet.PremiumSubscriberCount
                .GetValueOrDefault(0);

        /// <inheritdoc />
        public int PremiumTier
            => packet.PremiumTier.GetValueOrDefault(0);

        /// <inheritdoc />
        public async Task AddBanAsync(IDiscordGuildUser user, int pruneDays = 7, string reason = null)
        {
            await client.ApiClient.AddGuildBanAsync(Id, user.Id, pruneDays, reason);
        }

        /// <inheritdoc />
        public async Task<IDiscordRole> CreateRoleAsync(CreateRoleArgs roleParams = null)
            => await client.CreateRoleAsync(Id, roleParams);

        /// <inheritdoc />
        public async Task<IDiscordGuildChannel> GetChannelAsync(ulong id)
            => (await client.GetChannelAsync(id, Id)) as IDiscordGuildChannel;

        /// <inheritdoc />
        public async Task<IEnumerable<IDiscordGuildChannel>> GetChannelsAsync()
        {
            return await client.GetChannelsAsync(Id);
        }

        /// <inheritdoc />
        public Task<IDiscordChannel> GetDefaultChannelAsync()
        {
            if(!packet.SystemChannelId.HasValue)
            {
                return Task.FromResult<IDiscordChannel>(null);
            }

            return client.GetChannelAsync(packet.SystemChannelId.Value, Id);
        }

        /// <inheritdoc />
        public Task<IDiscordGuildUser> GetMemberAsync(ulong id)
        {
            return client.GetGuildUserAsync(id, Id);
        }

        /// <inheritdoc />
        public Task<IEnumerable<IDiscordGuildUser>> GetMembersAsync()
        {
            return client.GetGuildUsersAsync(Id);
        }

        /// <inheritdoc/>
        public Task<IDiscordGuildUser> GetOwnerAsync()
            => GetMemberAsync(OwnerId);

        /// <inheritdoc />
        public async Task<GuildPermission> GetPermissionsAsync(IDiscordGuildUser user)
        {
            if(user.Id == OwnerId)
            {
                return GuildPermission.All;
            }

            GuildPermission permissions = Permissions;

            if(permissions.HasFlag(GuildPermission.Administrator))
            {
                return GuildPermission.All;
            }

            IDiscordRole everyoneRole = await GetRoleAsync(Id);
            permissions = everyoneRole.Permissions;

            if(user.RoleIds != null)
            {
                var roles = await GetRolesAsync();
                foreach(IDiscordRole role in roles.Where(x => user.RoleIds.Contains(x.Id)))
                {
                    permissions |= role.Permissions;
                }
            }

            if(permissions.HasFlag(GuildPermission.Administrator))
            {
                return GuildPermission.All;
            }
            return permissions;
        }

        /// <inheritdoc />
        public Task<int> GetPruneCountAsync(int days)
        {
            return client.ApiClient.GetPruneCountAsync(Id, days);
        }

        /// <inheritdoc />
        public async Task<IDiscordRole> GetRoleAsync(ulong id)
            => await client.GetRoleAsync(Id, id)
                .ConfigureAwait(false);

        /// <inheritdoc />
        public async Task<IEnumerable<IDiscordRole>> GetRolesAsync()
            => await client.GetRolesAsync(Id)
                .ConfigureAwait(false);

        /// <inheritdoc />
        public async Task<IDiscordGuildUser> GetSelfAsync()
        {
            IDiscordUser user = await client.GetSelfAsync()
                .ConfigureAwait(false);

            if(user == null)
            {
                throw new InvalidOperationException("Could not find self user");
            }

            return await GetMemberAsync(user.Id);
        }

        /// <inheritdoc />
        public async Task<int?> PruneMembersAsync(int days, bool computeCount = false)
        {
            // NOTE: It is not recommended to compute these counts for large guilds.
            if(computeCount && MemberCount > 1000)
            {
                computeCount = false;
            }

            return await client.ApiClient.PruneGuildMembersAsync(Id, days, computeCount);
        }

        /// <inheritdoc />
        public async Task RemoveBanAsync(IDiscordGuildUser user)
        {
            if(user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            await client.ApiClient.RemoveGuildBanAsync(Id, user.Id);
        }
    }
}