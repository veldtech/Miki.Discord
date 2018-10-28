using Miki.Discord.Common;
using Miki.Discord.Common.Packets;
using System;
using System.Threading.Tasks;

namespace Miki.Discord.Internal
{
	public class DiscordUser : IDiscordUser
	{
		private DiscordClient _client;
		private DiscordUserPacket _user;

		public DiscordUser()
		{
		}

		public DiscordUser(DiscordUserPacket packet, DiscordClient client)
		{
			_client = client;
			_user = packet;
		}

		public string Username
			=> _user.Username;

		public string Discriminator
			=> _user.Discriminator;

		public bool IsBot
			=> _user.IsBot;

		public ulong Id
			=> _user.Id;

		public string AvatarId
			=> _user.Avatar;

		public string GetAvatarUrl(ImageType type = ImageType.AUTO, ImageSize size = ImageSize.x256)
			=> DiscordHelper.GetAvatarUrl(_user, type, size);

		public string Mention
			=> $"<@{Id}>";

		public async Task<IDiscordPresence> GetPresenceAsync()
			=> await _client.GetUserPresence(Id);

		public DateTimeOffset CreatedAt
			=> this.GetCreationTime();

		public async Task<IDiscordTextChannel> GetDMChannelAsync()
			=> await _client.CreateDMAsync(Id);
	}
}