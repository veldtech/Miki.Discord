using Miki.Discord.Common;
using Miki.Discord.Common.Packets;
using Xunit;

namespace Miki.Discord.Tests
{
	public class Helpers
	{
		public class User
		{
			DiscordUserPacket user;

			public User()
			{
				user = new DiscordUserPacket()
				{
					Id = 111,
					Discriminator = "1234"
				};
			}

			[Fact]
			public void AvatarStatic()
			{
				user.Avatar = "2345243f3oim4foi34mf3k4f";

				Assert.Equal("https://cdn.discordapp.com/avatars/111/2345243f3oim4foi34mf3k4f.png?size=256", DiscordUtils.GetAvatarUrl(user));
				Assert.Equal("https://cdn.discordapp.com/avatars/111/2345243f3oim4foi34mf3k4f.webp?size=2048", DiscordUtils.GetAvatarUrl(user, ImageType.WEBP, ImageSize.x2048));
				Assert.Equal("https://cdn.discordapp.com/avatars/111/2345243f3oim4foi34mf3k4f.jpeg?size=16", DiscordUtils.GetAvatarUrl(user, ImageType.JPEG, ImageSize.x16));
			}

			[Fact]
			public void AvatarAnimated()
			{
				user.Avatar = "a_owiejfowiejf432ijf3o";

				Assert.Equal("https://cdn.discordapp.com/avatars/111/a_owiejfowiejf432ijf3o.gif?size=256", DiscordUtils.GetAvatarUrl(user));
				Assert.Equal("https://cdn.discordapp.com/avatars/111/a_owiejfowiejf432ijf3o.webp?size=2048", DiscordUtils.GetAvatarUrl(user, ImageType.WEBP, ImageSize.x2048));
				Assert.Equal("https://cdn.discordapp.com/avatars/111/a_owiejfowiejf432ijf3o.jpeg?size=16", DiscordUtils.GetAvatarUrl(user, ImageType.JPEG, ImageSize.x16));
			}

			[Fact]
			public void AvatarNull()
			{
				user.Avatar = null;

				Assert.Equal($"https://cdn.discordapp.com/embed/avatars/{ushort.Parse(user.Discriminator) % 5}.png", DiscordUtils.GetAvatarUrl(user));
				Assert.Equal($"https://cdn.discordapp.com/embed/avatars/{ushort.Parse(user.Discriminator) % 5}.png", DiscordUtils.GetAvatarUrl(user, ImageType.PNG, ImageSize.x512));
			}
		}
	}
}
