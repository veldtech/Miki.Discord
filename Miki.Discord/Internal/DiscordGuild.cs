using Miki.Discord.Common;
using Miki.Discord.Common.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miki.Discord.Internal
{
	public class DiscordGuild : IDiscordGuild
	{
		private DiscordGuildPacket _packet;
		private DiscordClient _client;

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
			=> DiscordHelper.GetAvatarUrl(Id, _packet.Icon);

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

		public async Task AddBanAsync(IDiscordGuildUser user, int pruneDays = 7, string reason = null)
		{
			await _client.ApiClient.AddGuildBanAsync(Id, user.Id, pruneDays, reason);
		}

		public async Task<IDiscordRole> CreateRoleAsync(CreateRoleArgs roleParams = null)
			=> await _client.CreateRoleAsync(Id, roleParams);

		public async Task<IDiscordGuildChannel> GetChannelAsync(ulong id)
		{
			return await _client.GetChannelAsync(id, Id) as IDiscordGuildChannel;
		}

		public async Task<IReadOnlyList<IDiscordGuildChannel>> GetChannelsAsync()
		{
			return await _client.GetChannelsAsync(Id);
		}

		public IDiscordChannel GetDefaultChannel()
		{
			return Channels.FirstOrDefault(x => x.Id == _packet.SystemChannelId);
		}

		public async Task<IDiscordChannel> GetDefaultChannelAsync()
		{
			if (!_packet.SystemChannelId.HasValue)
			{
				return null;
			}

			return await _client.GetChannelAsync(_packet.SystemChannelId.Value, Id);
		}

		public async Task<IDiscordGuildUser> GetMemberAsync(ulong id)
		{
			DiscordGuildMemberPacket guildMemberPacket = await _client.GetGuildMemberPacketAsync(id, Id);
			DiscordUserPacket userPacket = await _client.GetUserPacketAsync(id);

			return new DiscordGuildUser(guildMemberPacket, _client, this);
		}

		public async Task<IDiscordGuildUser[]> GetMembersAsync()
		{
			return (await _client.CacheClient.HashValuesAsync<DiscordGuildMemberPacket>(CacheUtils.GuildMembersKey(Id)))
				.Select(x => new DiscordGuildUser(x, _client, this))
				.ToArray();
		}

		public async Task<IDiscordGuildUser> GetOwnerAsync()
		{
			return await GetMemberAsync(OwnerId);
		}

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
				foreach (IDiscordRole role in Roles.Where(x => user.RoleIds.Contains(x.Id)))
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

		public async Task<IDiscordRole> GetRoleAsync(ulong id)
			=> await _client.GetRoleAsync(Id, id);

		public async Task<IReadOnlyList<IDiscordRole>> GetRolesAsync()
			=> await _client.GetRolesAsync(Id);

		public async Task<IDiscordGuildUser> GetSelfAsync()
		{
			// TODO: maybe optimize this one.
			var me = await _client.GetCurrentUserAsync();
			return await GetMemberAsync(me.Id);
		}

		public async Task RemoveBanAsync(IDiscordGuildUser user)
			=> await _client.ApiClient.RemoveGuildBanAsync(Id, user.Id);
	}
}
