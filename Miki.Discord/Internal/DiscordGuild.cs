using Miki.Discord.Common;
using Miki.Discord.Common.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Miki.Discord.Internal
{
	public class DiscordGuild : IDiscordGuild
	{
		private readonly DiscordGuildPacket _packet;
		private readonly DiscordClient _client;

		public DiscordGuild(DiscordGuildPacket packet, DiscordClient client)
		{
			_packet = packet;
			_client = client;
		}

		public string Name
			=> _packet.Name;

		public ulong Id
			=> _packet.Id;

		public string IconUrl
			=> DiscordUtils.GetAvatarUrl(Id, _packet.Icon);

		public ulong OwnerId
			=> _packet.OwnerId;

		public int MemberCount
			=> _packet.MemberCount ?? 0;

		public GuildPermission Permissions
			=> (GuildPermission)_packet.Permissions.GetValueOrDefault(0);

		public IReadOnlyCollection<IDiscordGuildChannel> Channels
			=> _packet.Channels.Select(x => new DiscordGuildChannel(x, _client)).ToList();

		public IReadOnlyCollection<IDiscordRole> Roles
			=> _packet.Roles.Select(x => new DiscordRole(x, _client)).ToList();

        public int PremiumSubscriberCount 
            => _packet.PremiumSubscriberCount;

        public int PremiumTier 
            => _packet.PremiumTier;

        public async Task AddBanAsync(IDiscordGuildUser user, int pruneDays = 7, string reason = null)
		{
			await _client.ApiClient.AddGuildBanAsync(Id, user.Id, pruneDays, reason);
		}

		public async Task<IDiscordRole> CreateRoleAsync(CreateRoleArgs roleParams = null)
			=> await _client.CreateRoleAsync(Id, roleParams);

		public Task<IDiscordGuildChannel> GetChannelAsync(ulong id)
		    => _client.GetChannelAsync<IDiscordGuildChannel>(id, Id);

		public async Task<IEnumerable<IDiscordGuildChannel>> GetChannelsAsync()
		{
			return await _client.GetChannelsAsync(Id);
		}

		public IDiscordChannel GetDefaultChannel()
		{
			return Channels.FirstOrDefault(x => x.Id == _packet.SystemChannelId);
		}

		public Task<IDiscordChannel> GetDefaultChannelAsync()
		{
			if (!_packet.SystemChannelId.HasValue)
			{
				return null;
			}
			return _client.GetChannelAsync(_packet.SystemChannelId.Value, Id);
		}

		public async Task<IDiscordGuildUser> GetMemberAsync(ulong id)
		{
			DiscordGuildMemberPacket guildMemberPacket = await _client.GetGuildMemberPacketAsync(id, Id);
			return new DiscordGuildUser(guildMemberPacket, _client);
		}

		public async Task<IEnumerable<IDiscordGuildUser>> GetMembersAsync()
		{
            return (await _client.CacheClient.HashValuesAsync<DiscordGuildMemberPacket>(CacheUtils.GuildMembersKey(Id)))
                .Select(x => new DiscordGuildUser(x, _client));
        }

		public Task<IDiscordGuildUser> GetOwnerAsync()
			=> GetMemberAsync(OwnerId);

		public async Task<GuildPermission> GetPermissionsAsync(IDiscordGuildUser user)
		{
			if (user.Id == OwnerId)
			{
				return GuildPermission.All;
			}

			GuildPermission permissions = Permissions;

			if (permissions.HasFlag(GuildPermission.Administrator))
			{
				return GuildPermission.All;
			}

			IDiscordRole everyoneRole = await GetRoleAsync(Id);
			permissions = everyoneRole.Permissions;

			if (user.RoleIds != null)
			{
                var roles = await GetRolesAsync();
				foreach (IDiscordRole role in roles.Where(x => user.RoleIds.Contains(x.Id)))
				{
					permissions |= role.Permissions;
				}
			}

			if (permissions.HasFlag(GuildPermission.Administrator))
			{
				return GuildPermission.All;
			}
			return permissions;
		}

        public Task<int> GetPruneCountAsync(int days)
        {
            return _client.ApiClient.GetPruneCountAsync(Id, days);
        }

        public async Task<IDiscordRole> GetRoleAsync(ulong id)
            => await _client.GetRoleAsync(Id, id)
                .ConfigureAwait(false);

		public async Task<IEnumerable<IDiscordRole>> GetRolesAsync()
			=> await _client.GetRolesAsync(Id)
                .ConfigureAwait(false);

        public async Task<IDiscordGuildUser> GetSelfAsync()
		{
            IDiscordUser user = await _client.GetSelfAsync()
                .ConfigureAwait(false);

            if(user == null)
            {
                throw new InvalidOperationException("Could not find self user");
            }

			return await GetMemberAsync(user.Id);
		}

        public async Task<int?> PruneMembersAsync(int days, bool computeCount = false)
        {
            // NOTE: It is not recommended to compute these counts for large guilds.
            if(computeCount && MemberCount > 1000)
            {
                computeCount = false;
            }
            return await _client.ApiClient.PruneGuildMembersAsync(Id, days, computeCount);
        }

        public async Task RemoveBanAsync(IDiscordGuildUser user)
        {
            if(user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            await _client.ApiClient.RemoveGuildBanAsync(Id, user.Id);
        }
    }
}