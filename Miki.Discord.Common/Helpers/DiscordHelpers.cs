using Miki.Discord.Common.Packets;
using System;

namespace Miki.Discord.Common
{
    /// <summary>
    /// Helper methods and properties for Discord related objects.
    /// </summary>
    public static class DiscordHelpers
    {
        /// <summary>
        /// API base path.
        /// </summary>
        public const string BasePath = "/api/v6";

        /// <summary>
        /// Discord's snowflake start epoch. Used to find the creation date of a snowflake.
        /// </summary>
        public const long DiscordEpoch = 1420070400000;

        /// <summary>
        /// Discord base URL. Links to the website.
        /// </summary>
        public const string DiscordUrl = "https://discord.com";

        /// <summary>
        /// CDN base URL. Used to fetch resources from Discord's CDN.
        /// </summary>
        public const string CdnUrl = "https://cdn.discordapp.com";

        /// <summary>
        /// Helper function to automatically get either custom or default avatar based on the
        /// values received from a <see cref="DiscordUserPacket"/>.
        /// </summary>
        public static string GetAvatarUrl(
            DiscordUserPacket packet, 
            ImageType type = ImageType.AUTO, 
            ImageSize size = ImageSize.x256)
        {
            if(packet.Avatar != null)
            {
                return GetAvatarUrl(packet.Id, packet.Avatar, type, size);
            }

            if(short.TryParse(packet.Discriminator, out var discriminator))
            {
                return GetAvatarUrl(discriminator);
            }
            return GetAvatarUrl(1);
        }

        /// <summary>
        /// Gets user's custom avatar URL.
        /// </summary>
        public static string GetAvatarUrl(
            ulong id, 
            string hash, 
            ImageType imageType = ImageType.AUTO, 
            ImageSize size = ImageSize.x256)
        {
            if(imageType == ImageType.AUTO)
            {
                imageType = hash.StartsWith("a_") ? ImageType.GIF : ImageType.PNG;
            }

            return $"{CdnUrl}/avatars/{id}/{hash}.{imageType.ToString().ToLower()}?size={(int)size}";
        }

        /// <summary>
        /// Gets the default Discord avatars based on the user's discriminator.
        /// </summary>
        public static string GetAvatarUrl(short discriminator)
            => $"{CdnUrl}/embed/avatars/{discriminator % 5}.png";

        /// <summary>
        /// Get a guild icon URL
        /// </summary>
        public static string GetIconUrl(
            DiscordGuildPacket packet,
            ImageType type = ImageType.AUTO,
            ImageSize size = ImageSize.x128)
        {
            if (type == ImageType.AUTO)
            {
                type = packet.Icon.StartsWith("a_") ? ImageType.GIF : ImageType.PNG;
            }

            var imgType = type.ToString().ToLowerInvariant();
            return $"{CdnUrl}/icons/{packet.Id}/{packet.Icon}.{imgType}?size={(int)size}";
        }

        /// <summary>
        /// Gets the time of creation for a snowflake.
        /// </summary>
        public static DateTime GetCreationTime(this ISnowflake snowflake)
        {
            return DateTimeOffset.FromUnixTimeSeconds(DiscordEpoch)
                .AddMilliseconds(snowflake.Id >> 22)
                .UtcDateTime;
        }
    }
}