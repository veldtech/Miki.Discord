namespace Miki.Discord.Internal
{
    using Miki.Discord.Common;
    using Miki.Discord.Common.Packets;
    using System;
    using System.Threading.Tasks;

    public class DiscordUser : IDiscordUser
    {
        private readonly DiscordUserPacket user;

        protected readonly IDiscordClient client;

        public DiscordUser(DiscordUserPacket packet, IDiscordClient client)
        {
            this.client = client;
            user = packet;
        }

        public string Username
            => user.Username;

        public string Discriminator
            => user.Discriminator;

        public bool IsBot
            => user.IsBot;

        public ulong Id
            => user.Id;

        public string AvatarId
            => user.Avatar;

        public string GetAvatarUrl(ImageType type = ImageType.AUTO, ImageSize size = ImageSize.x256)
            => DiscordUtils.GetAvatarUrl(user, type, size);

        public string Mention
            => $"<@{Id}>";

        public async Task<IDiscordPresence> GetPresenceAsync()
            => await client.GetUserPresence(Id);

        public DateTimeOffset CreatedAt
            => this.GetCreationTime();

        public async Task<IDiscordTextChannel> GetDMChannelAsync()
        {
            var currentUser = await client.GetSelfAsync();
            if(Id == currentUser.Id)
            {
                throw new InvalidOperationException("Can't create a DM channel with self.");
            }
            return await client.CreateDMAsync(Id);
        }
    }
}