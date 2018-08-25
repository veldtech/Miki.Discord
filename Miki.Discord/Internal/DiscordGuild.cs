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
			=> _client.GetUserAvatarUrl(Id, _packet.Icon);

		public ulong OwnerId
			=> _packet.OwnerId;

		public GuildPermission Permissions 
			=> (GuildPermission)_packet.Permissions.GetValueOrDefault(0);

		public IReadOnlyCollection<IDiscordGuildChannel> Channels 
			=> _packet.Channels.Select(x => new DiscordGuildChannel(x, _client)).ToList();

		public IReadOnlyCollection<IDiscordGuildUser> Members 
			=> _packet.Members.Select(x => new DiscordGuildUser(x, _client, this)).ToList();

		public IReadOnlyCollection<IDiscordRole> Roles 
			=> _packet.Roles.Select(x => new DiscordRole(x, _client)).ToList();

		public async Task AddBanAsync(IDiscordGuildUser user, int pruneDays = 7, string reason = null)
		{
			await _client.AddBanAsync(Id, user.Id, pruneDays, reason);
		}

		public async Task<IDiscordRole> CreateRoleAsync(CreateRoleArgs roleParams = null)
			=> await _client.CreateRoleAsync(Id, roleParams);

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

			return await _client.GetChannelAsync(_packet.SystemChannelId.Value);
		}

		public IDiscordGuildUser GetMember(ulong id)
		{
			DiscordGuildMemberPacket guildMemberPacket = _packet.Members.FirstOrDefault(x => x.User.Id == id);

			if (guildMemberPacket == null)
			{
				return null;
			}

			return new DiscordGuildUser(guildMemberPacket, _client, this);
		}

		public IDiscordGuildUser GetOwner()
		{
			return Members.FirstOrDefault(x => x.Id == OwnerId);
		}

		public GuildPermission GetPermissions(IDiscordGuildUser user)
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

			IDiscordRole everyoneRole = GetRole(Id);
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

		public IDiscordRole GetRole(ulong id)
		{
			return Roles.FirstOrDefault(x => x.Id == id);
		}

		public async Task<IDiscordGuildUser> GetSelfAsync()
		{
			var me = await _client.GetCurrentUserAsync();
			return Members.FirstOrDefault(x => x.Id == me.Id);
		}

		public IReadOnlyCollection<IDiscordChannel> GetTextChannels()
		{
			return _packet.Channels
				.Where(x => x.Type == ChannelType.GUILDTEXT)
				.Select(x => new DiscordGuildChannel(x, _client)).ToList();
		}

		public IReadOnlyCollection<IDiscordChannel> GetVoiceChannels()
		{
			return null;
		}

		public async Task RemoveBanAsync(IDiscordGuildUser user)
			=> await _client.RemoveBanAsync(Id, user.Id);
	}
}
