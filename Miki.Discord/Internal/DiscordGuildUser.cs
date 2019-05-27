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

		public DiscordGuildUser(DiscordGuildMemberPacket packet, DiscordClient client)
		{
			_packet = packet;
			_client = client;
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
			await _client.ApiClient.AddGuildMemberRoleAsync(GuildId, Id, role.Id);
		}

		public string GetAvatarUrl(ImageType type = ImageType.AUTO, ImageSize size = ImageSize.x256)
			=> DiscordUtils.GetAvatarUrl(_packet.User, type, size);

		public async Task<IDiscordTextChannel> GetDMChannelAsync()
			=> await _client.CreateDMAsync(_packet.User.Id);

		public async Task<IDiscordPresence> GetPresenceAsync()
			=> await _client.GetUserPresence(_packet.User.Id);

		public async Task<IDiscordGuild> GetGuildAsync()
			=> await _client.GetGuildAsync(_packet.GuildId);

		public async Task KickAsync(string reason = null)
		{
			await _client.ApiClient.RemoveGuildMemberAsync(GuildId, Id, reason);
		}

		public async Task RemoveRoleAsync(IDiscordRole role)
		{
			await _client.ApiClient.RemoveGuildMemberRoleAsync(GuildId, Id, role.Id);
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