using Miki.Cache;
using Miki.Cache.InMemory;
using Miki.Discord.Caching;
using Miki.Discord.Common;
using Miki.Discord.Common.Packets;
using Miki.Discord.Mocking;
using Miki.Discord.Tests.Dummy;
using Miki.Serialization.Protobuf;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Miki.Discord.Tests
{
	/// <summary>
	/// Contains tests for Caching
	/// </summary>
    public class Caching
    {
		readonly DummyGateway gateway;
		CacheClient cache;
		ICachePool pool;

		DiscordChannelPacket channel;
		DiscordGuildPacket guild;
		DiscordGuildMemberPacket member;
		DiscordUserPacket user;

		public Caching()
		{
			gateway = new DummyGateway();

			ResetObjects();
		}

		void ResetObjects()
		{
			pool = new InMemoryCachePool(new ProtobufSerializer());

			cache = new CacheClient(
				gateway,
				pool,
				new DummyApiClient()
			);

			channel = new DiscordChannelPacket
			{
				GuildId = 123,
				Id = 111,
				IsNsfw = true,
				Name = "test channel",
				Type = ChannelType.GUILDTEXT,
				CreatedAt = 0,
			};

			user = new DiscordUserPacket
			{
				Avatar = "avatar",
				Discriminator = "1111",
				Email = null,
				Id = 1023,
				IsBot = false,
				MfaEnabled = false,
				Username = "test user",
				Verified = false,
			};

			member = new DiscordGuildMemberPacket
			{
				GuildId = 123,
				JoinedAt = 0,
				Deafened = false,
				Muted = false,
				Nickname = null,
				Roles = new List<ulong>(),
				User = user
			};

			guild = new DiscordGuildPacket
			{
				Channels = new List<DiscordChannelPacket> { channel },
				DefaultMessageNotifications = 1,
				Emojis = new List<DiscordEmojiPacket>(),
				ExplicitContentFilter = 1,
				Features = new List<string>(),
				Icon = "this-is-a-icon.png",
				Id = 123,
				IsLarge = true,
				IsOwner = true,
				MemberCount = 1,
				Members = new List<DiscordGuildMemberPacket>(),
				MFALevel = 1,
				Name = "test guild",
				Presences = new List<DiscordPresencePacket>(),
				Region = "lul region",
				Roles = new List<DiscordRolePacket>(),
			};

			pool.Get.UpsertAsync($"discord:channel:{channel.Id}", channel).GetAwaiter().GetResult();
			pool.Get.UpsertAsync($"discord:guild:{guild.Id}", guild).GetAwaiter().GetResult();
		}

		[Fact]
		public async Task UserUpdateAsync()
		{
			ResetObjects();

			await gateway.OnGuildMemberAdd(member);

			user.Avatar = "new avi";

			await gateway.OnUserUpdate(user);

			DiscordGuildPacket newGuild = await pool.Get.GetAsync<DiscordGuildPacket>($"discord:guild:{guild.Id}");
			DiscordUserPacket currentUser = await pool.Get.GetAsync<DiscordUserPacket>($"discord:user:{user.Id}");

			Assert.NotEmpty(newGuild.Members);

			DiscordGuildMemberPacket newUser = newGuild.Members[0];

			Assert.Equal("new avi", currentUser.Avatar);
			Assert.Equal("new avi", newUser.User.Avatar);
		}

		[Fact]
		public async Task GuildMemberAddAsync()
		{
			ResetObjects();

			Assert.Empty(guild.Members);

			await gateway.OnGuildMemberAdd(member);

			DiscordGuildPacket g = await pool.Get.GetAsync<DiscordGuildPacket>($"discord:guild:{guild.Id}");

			Assert.NotEmpty(g.Members);

			HashSet<ulong> guildsJoined = await pool.Get.GetAsync<HashSet<ulong>>($"discord:user:{user.Id}:guilds");

			Assert.NotEmpty(guildsJoined);
			Assert.Collection(guildsJoined, x => Assert.Equal(guild.Id, x));
		}

		[Fact]
        public async Task ChannelUpdateAsync()
        {
			ResetObjects();

			Assert.True(channel.IsNsfw);
			Assert.Equal("test channel", channel.Name);

			channel.IsNsfw = false;
			channel.Name = "new test channel";

			await gateway.OnChannelUpdate(channel);

			DiscordChannelPacket newChannel = await pool.Get.GetAsync<DiscordChannelPacket>($"discord:channel:{channel.Id}");
			Assert.NotNull(newChannel);

			DiscordGuildPacket newGuild = await pool.Get.GetAsync<DiscordGuildPacket>($"discord:guild:{guild.Id}");
			Assert.NotNull(newGuild);

			Assert.False(newChannel.IsNsfw);
			Assert.False(newGuild.Channels[0].IsNsfw);

			Assert.Equal("new test channel", newChannel.Name);
			Assert.Equal("new test channel", newGuild.Channels[0].Name);
		}
	}
}
