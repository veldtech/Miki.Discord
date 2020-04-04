namespace Miki.Discord.Common
{
    using System;
    using System.Threading.Tasks;

    public interface IDiscordUser : ISnowflake
    {
        string AvatarId { get; }

        string Mention { get; }

        string Username { get; }

        string Discriminator { get; }

        DateTimeOffset CreatedAt { get; }

        bool IsBot { get; }

        Task<IDiscordPresence> GetPresenceAsync();

        Task<IDiscordTextChannel> GetDMChannelAsync();

        string GetAvatarUrl(ImageType type = ImageType.AUTO, ImageSize size = ImageSize.x256);
    }
}