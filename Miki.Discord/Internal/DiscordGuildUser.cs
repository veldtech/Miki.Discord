using Miki.Discord.Common;
using Miki.Discord.Common.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miki.Discord.Internal
{
	public class DiscordGuildUser : IDiscordGuildUser
    {
		DiscordGuildMemberPacket _packet;
		DiscordClient _client;
		IDiscordGuild _guild;

		public DiscordGuildUser(DiscordGuildMemberPacket packet, DiscordUserPacket user, DiscordClient client, IDiscordGuild guild)
		{
			_packet = packet;

			if (user != null)
			{
				_packet.User = user;
			}

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
			=> new DateTimeOffset();

		public int Hierarchy
		{
			get
			{
				if (RoleIds.Count > 0)
				{
					return _guild.Roles
					  .Where(x => RoleIds.Contains(x.Id))
					  .Max(x => x.Position);
				}
				return 0;
			}
		}

		public DateTimeOffset CreatedAt 
			=> new DateTimeOffset((long)Id >> 22, TimeSpan.FromTicks(1420070400000));

		public async Task AddRoleAsync(IDiscordRole role)
			=> await _client.AddGuildMemberRoleAsync(GuildId, Id, role.Id);

		public string GetAvatarUrl(ImageType type = ImageType.AUTO, ImageSize size = ImageSize.x256)
			=> DiscordHelper.GetAvatarUrl(_packet.User, type, size);

		public async Task<IDiscordChannel> GetDMChannelAsync()
			=> await _client.CreateDMAsync(_packet.User.Id);

		public async Task<IDiscordPresence> GetPresenceAsync()
			=> await _client.GetUserPresence(_packet.User.Id);

		public async Task<IDiscordGuild> GetGuildAsync()
			=> await _client.GetGuildAsync(_packet.GuildId);

		public async Task KickAsync(string reason = null)
			=> await _client.RemoveGuildMemberAsync(GuildId, Id, reason);

		public async Task RemoveRoleAsync(IDiscordRole role)
			=> await _client.RemoveGuildMemberRoleAsync(GuildId, Id, role.Id);
	}
}
