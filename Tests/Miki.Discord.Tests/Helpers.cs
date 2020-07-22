using Miki.Discord.Common;
using Miki.Discord.Common.Packets;
using Xunit;

namespace Miki.Discord.Tests
{
    public class Helpers
    {
        public class User
        {
            private readonly DiscordUserPacket user;

            public User()
            {
                user = new DiscordUserPacket()
                {
                    Id = 111,
                    Discriminator = 1234
                };
            }

            [Fact]
            public void AvatarStatic()
            {
                user.Avatar = "2345243f3oim4foi34mf3k4f";

                Assert.Equal(
                    "https://cdn.discordapp.com/avatars/111/2345243f3oim4foi34mf3k4f.png?size=256", 
                    DiscordHelpers.GetAvatarUrl(user));
                Assert.Equal(
                    "https://cdn.discordapp.com/avatars/111/2345243f3oim4foi34mf3k4f.webp?size=2048", 
                    DiscordHelpers.GetAvatarUrl(user, ImageType.WEBP, ImageSize.x2048));
                Assert.Equal("https://cdn.discordapp.com/avatars/111/2345243f3oim4foi34mf3k4f.jpeg?size=16", 
                    DiscordHelpers.GetAvatarUrl(user, ImageType.JPEG, ImageSize.x16));
            }

            [Fact]
            public void AvatarAnimated()
            {
                user.Avatar = "a_owiejfowiejf432ijf3o";

                Assert.Equal(
                    "https://cdn.discordapp.com/avatars/111/a_owiejfowiejf432ijf3o.gif?size=256", 
                    DiscordHelpers.GetAvatarUrl(user));
                Assert.Equal(
                    "https://cdn.discordapp.com/avatars/111/a_owiejfowiejf432ijf3o.webp?size=2048",
                    DiscordHelpers.GetAvatarUrl(user, ImageType.WEBP, ImageSize.x2048));
                Assert.Equal(
                    "https://cdn.discordapp.com/avatars/111/a_owiejfowiejf432ijf3o.jpeg?size=16", 
                    DiscordHelpers.GetAvatarUrl(user, ImageType.JPEG, ImageSize.x16));
            }

            [Fact]
            public void AvatarNull()
            {
                user.Avatar = null;

                Assert.Equal
                    ($"https://cdn.discordapp.com/embed/avatars/{user.Discriminator % 5}.png", 
                    DiscordHelpers.GetAvatarUrl(user));
                Assert.Equal(
                    $"https://cdn.discordapp.com/embed/avatars/{user.Discriminator % 5}.png", 
                    DiscordHelpers.GetAvatarUrl(user, ImageType.PNG, ImageSize.x512));
            }
        }
    }
}
