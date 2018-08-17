using Miki.Discord.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miki.Discord.Internal
{
	public class DiscordGuildChannel : DiscordChannel, IDiscordGuildChannel
	{
		public DiscordGuildChannel(DiscordChannelPacket packet, DiscordClient client)
			:base(packet, client)
		{
		}

		public ulong GuildId
			=> _packet.GuildId.Value;

		public ChannelType Type
			=> _packet.Type;

		public async Task<IDiscordGuild> GetGuildAsync()
			=> await _client.GetGuildAsync(GuildId);

		public async Task<GuildPermission> GetPermissionsAsync(IDiscordGuildUser user)
		{
			IDiscordGuild guild = await GetGuildAsync();

			GuildPermission permissions = guild.GetPermissions(user);
			
			if(permissions.HasFlag(GuildPermission.Administrator))
			{
				return GuildPermission.All;
			}

			if (_packet.PermissionOverwrites != null)
			{
				PermissionOverwrite overwriteEveryone = _packet.PermissionOverwrites
					.FirstOrDefault(x => x.Id == GuildId) ?? null;

				if (overwriteEveryone != null)
				{
					permissions &= ~overwriteEveryone.DeniedPermissions;
					permissions |= overwriteEveryone.AllowedPermissions;
				}

				PermissionOverwrite overwrites = new PermissionOverwrite();

				if (user.RoleIds != null)
				{
					foreach (ulong roleId in user.RoleIds)
					{
						PermissionOverwrite roleOverwrites = _packet.PermissionOverwrites.FirstOrDefault(x => x.Id == roleId);

						if (roleOverwrites != null)
						{
							overwrites.AllowedPermissions |= roleOverwrites.AllowedPermissions;
							overwrites.DeniedPermissions &= roleOverwrites.DeniedPermissions;
						}
					}
				}

				permissions &= ~overwrites.DeniedPermissions;
				permissions |= overwrites.AllowedPermissions;

				PermissionOverwrite userOverwrite = _packet.PermissionOverwrites.FirstOrDefault(x => x.Id == user.Id);

				if (userOverwrite != null)
				{
					permissions &= ~userOverwrite.DeniedPermissions;
					permissions |= userOverwrite.AllowedPermissions;
				}
			}

			return permissions;
		}

		public async Task<IDiscordGuildUser> GetUserAsync(ulong id)
			=> await _client.GetGuildUserAsync(id, GuildId);
	}
}
