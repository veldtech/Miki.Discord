using Miki.Discord.Common;
using Miki.Discord.Rest.Entities;
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
			=> _packet.GuildId;

		public bool IsNsfw
			=> _packet.IsNsfw;

		public string Name
			=> _packet.Name;

		public ulong Id
			=> _packet.Id;

		public async Task<IDiscordGuild> GetGuildAsync()
			=> await _client.GetGuildAsync(GuildId);

		public async Task<GuildPermission> GetPermissionsAsync(IDiscordGuildUser user)
		{
			IDiscordGuild guild = await GetGuildAsync();

			GuildPermission permissions = await guild.GetPermissionsAsync(user);
			
			if(permissions.HasFlag(GuildPermission.Administrator))
			{
				return GuildPermission.All;
			}

			PermissionOverwrite overwriteEveryone = _packet.PermissionOverwrites
				.FirstOrDefault(x => x.Id == GuildId);

			if(overwriteEveryone != null)
			{
				permissions &= ~overwriteEveryone.DeniedPermissions;
				permissions |= overwriteEveryone.AllowedPermissions;
			}

			PermissionOverwrite overwrites = new PermissionOverwrite();

			foreach(ulong roleId in user.RoleIds)
			{
				PermissionOverwrite roleOverwrites = _packet.PermissionOverwrites.FirstOrDefault(x => x.Id == roleId);

				if(roleOverwrites != null)
				{
					overwrites.AllowedPermissions |= roleOverwrites.AllowedPermissions;
					overwrites.DeniedPermissions &= roleOverwrites.DeniedPermissions;
				}
			}

			permissions &= ~overwrites.DeniedPermissions;
			permissions |= overwrites.AllowedPermissions;

			PermissionOverwrite userOverwrite = _packet.PermissionOverwrites.FirstOrDefault(x => x.Id == user.Id);

			if(userOverwrite != null)
			{
				permissions &= ~userOverwrite.DeniedPermissions;
				permissions |= userOverwrite.AllowedPermissions;
			}

			return permissions;
		}

		public async Task<IDiscordGuildUser> GetUserAsync(ulong id)
			=> await _client.GetGuildUserAsync(id, GuildId);

		public async Task<IDiscordMessage> SendFileAsync(Stream file, string fileName, string content, bool isTTS = false, DiscordEmbed embed = null)
			=> await _client.SendFileAsync(Id, file, fileName, new MessageArgs
			{
				content = content,
				tts = isTTS,
				embed = embed
			});

		public async Task<IDiscordMessage> SendMessageAsync(string content, bool isTTS = false, DiscordEmbed embed = null)
			=> await _client.SendMessageAsync(Id, new MessageArgs
			{
				content = content,
				tts = isTTS,
				embed = embed
			}, true);
	}
}
