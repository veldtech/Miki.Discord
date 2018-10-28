using Miki.Discord.Common;
using Miki.Discord.Common.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Miki.Discord.Internal
{
	public class DiscordGuildUser : IDiscordGuildUser
	{
		private DiscordGuildMemberPacket _packet;
		private DiscordClient _client;
		private IDiscordGuild _guild;

		public DiscordGuildUser(DiscordGuildMemberPacket packet, DiscordClient client, IDiscordGuild guild)
		{
			_packet = packet;
			_client = client;
			_guild = guild;
		}

		public ulong Id
			=> _packet.User.Id;

		public string Username
			=> _packet.User.Username;

		public string Discriminator
			=> _packet.User.Discriminator;

		public bool IsBot
			=> _packet.User.IsBot;

		public string AvatarId
			=> _packet.User.Avatar;

		public string Mention
			=> $"<@{Id}>";

		public string Nickname
			=> _packet.Nickname;

		public IReadOnlyCollection<ulong> RoleIds
			=> _packet.Roles;

		public ulong GuildId
			=> _packet.GuildId;

		public DateTimeOffset JoinedAt
			=> new DateTimeOffset(_packet.JoinedAt, new TimeSpan(0));

		public DateTimeOffset CreatedAt
			=> this.GetCreationTime();

		public async Task AddRoleAsync(IDiscordRole role)
		{
			await _client._apiClient.AddGuildMemberRoleAsync(GuildId, Id, role.Id);
		}

		public string GetAvatarUrl(ImageType type = ImageType.AUTO, ImageSize size = ImageSize.x256)
			=> DiscordHelper.GetAvatarUrl(_packet.User, type, size);

		public async Task<IDiscordTextChannel> GetDMChannelAsync()
			=> await _client.CreateDMAsync(_packet.User.Id);

		public async Task<IDiscordPresence> GetPresenceAsync()
			=> await _client.GetUserPresence(_packet.User.Id);

		public async Task<IDiscordGuild> GetGuildAsync()
			=> _guild ?? await _client.GetGuildAsync(_packet.GuildId);

		public async Task KickAsync(string reason = null)
		{
			await _client._apiClient.RemoveGuildMemberAsync(GuildId, Id, reason);
		}

		public async Task RemoveRoleAsync(IDiscordRole role)
		{
			await _client._apiClient.RemoveGuildMemberRoleAsync(GuildId, Id, role.Id);
		}

		public async Task<bool> HasPermissionsAsync(GuildPermission permissions)
		{
			GuildPermission p = await _guild.GetPermissionsAsync(this);
			return p.HasFlag(permissions);
		}

		public async Task<int> GetHierarchyAsync()
		{
			return (await _guild.GetRolesAsync())
				.Where(x => RoleIds.Contains(x.Id))
				.Max(x => x.Position);
		}
	}
}