using Miki.Discord.Common;
using Miki.Discord.Common.Packets;
using System;
using System.Threading.Tasks;

namespace Miki.Discord.Internal
{
	public class DiscordUser : IDiscordUser
	{
		private readonly DiscordUserPacket _user;

		protected readonly IDiscordClient Client;

		public DiscordUser(DiscordUserPacket packet, IDiscordClient client)
		{
			Client = client;
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
			=> DiscordUtils.GetAvatarUrl(_user, type, size);

		public string Mention
			=> $"<@{Id}>";

		public async Task<IDiscordPresence> GetPresenceAsync()
			=> await Client.GetUserPresence(Id);

		public DateTimeOffset CreatedAt
			=> this.GetCreationTime();

		public async Task<IDiscordTextChannel> GetDMChannelAsync()
		{
			var currentUser = await Client.GetSelfAsync();
			if(Id == currentUser.Id)
			{
				throw new InvalidOperationException("Can't create a DM channel with self.");
			}
			return await Client.CreateDMAsync(Id);
		}
	}
}