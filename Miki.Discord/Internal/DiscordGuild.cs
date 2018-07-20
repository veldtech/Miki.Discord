using Miki.Discord.Common;
using Miki.Discord.Rest.Entities;
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
			=> _client.GetUserAvatarUrl(Id, _packet.Icon);

		public ulong OwnerId
			=> _packet.OwnerId;

		public GuildPermission Permissions 
			=> (GuildPermission)_packet.Permissions.GetValueOrDefault(0);

		public async Task AddBanAsync(IDiscordGuildUser user, int pruneDays = 7, string reason = null)
		{
			await _client.AddBanAsync(Id, user.Id);
		}

		public async Task<IDiscordRole> CreateRoleAsync(CreateRoleArgs roleParams = null)
			=> await _client.CreateRoleAsync(Id, roleParams);

		public async Task<IReadOnlyCollection<IDiscordGuildChannel>> GetChannelsAsync()
			=> await _client.GetChannelsAsync(Id);

		public async Task<IDiscordChannel> GetDefaultChannelAsync()
		{
			if (!_packet.SystemChannelId.HasValue)
			{
				return null;
			}

			return await _client.GetChannelAsync(_packet.SystemChannelId.Value);
		}

		public async Task<IDiscordGuildUser> GetOwnerAsync()
			=> await _client.GetGuildUserAsync(OwnerId, Id);

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
				foreach (IDiscordRole role in (await GetRolesAsync())
					.Where(x => user.RoleIds.Contains(x.Id)))
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
			=> (await _client.GetRolesAsync(Id)).FirstOrDefault(x => x.Id == id);

		public async Task<IReadOnlyCollection<IDiscordRole>> GetRolesAsync()
			=> await _client.GetRolesAsync(Id);

		public async Task<IDiscordGuildUser> GetSelfAsync()
			=> await _client.GetGuildUserAsync(
				(await _client.GetCurrentUserAsync()).Id, 
				Id
			);

		public async Task<IReadOnlyCollection<IDiscordChannel>> GetTextChannelsAsync()
		{
			throw new NotImplementedException();
		}

		public async Task<IDiscordGuildUser> GetUserAsync(ulong id)
		{
			return await _client.GetGuildUserAsync(id, Id);
		}

		public Task<IReadOnlyCollection<IDiscordGuildUser>> GetUsersAsync()
		{
			throw new NotImplementedException();
		}

		public Task<IReadOnlyCollection<IDiscordChannel>> GetVoiceChannelsAsync()
		{
			throw new NotImplementedException();
		}

		public async Task RemoveBanAsync(IDiscordGuildUser user)
			=> await _client.RemoveBanAsync(Id, user.Id);
	}
}
