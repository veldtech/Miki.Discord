using Miki.Cache;
using Miki.Cache.StackExchange;
using Miki.Discord.Caching;
using Miki.Discord.Caching.Stages;
using Miki.Discord.Common;
using Miki.Discord.Common.Packets;
using Miki.Discord.Mocking;
using Miki.Discord.Tests.Dummy;
using Miki.Serialization.Protobuf;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
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
		IExtendedCacheClient client;

		DiscordChannelPacket channel;
		DiscordGuildPacket guild;
		DiscordGuildMemberPacket member;
		DiscordUserPacket user;
		DiscordRolePacket role;

		public Caching()
		{
			gateway = new DummyGateway();

			ResetObjects();
		}

		void ResetObjects()
		{
			pool = new StackExchangeCachePool(new ProtobufSerializer(), "localhost");

			cache = new CacheClient(
				gateway,
				pool.GetAsync().Result as IExtendedCacheClient,
				new DummyApiClient()
			);

			client = pool.GetAsync().Result as IExtendedCacheClient;

			new BasicCacheStage().Initialize(cache);

			role = new DiscordRolePacket
			{
				Color = 2342,
				Id = 999,
				IsHoisted = true,
				Managed = false,
				Mentionable = true,
				Name = "test role",
				Permissions = 34234,
				Position = 0
			};

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
				Members = new List<DiscordGuildMemberPacket>() { member },
				MFALevel = 1,
				Name = "test guild",
				Presences = new List<DiscordPresencePacket>(),
				Region = "lul region",
				Roles = new List<DiscordRolePacket>(),
			};

			(pool.GetAsync().Result).UpsertAsync($"discord:channel:{channel.Id}", channel).GetAwaiter().GetResult();
			(pool.GetAsync().Result).UpsertAsync($"discord:guild:{guild.Id}", guild).GetAwaiter().GetResult();
		}

		[Fact]
		public async Task UserUpdateAsync()
		{
			ResetObjects();

			await gateway.OnGuildMemberRemove(guild.Id, member.User);

			DiscordGuildPacket newGuild = await client.GetAsync<DiscordGuildPacket>($"discord:guild:{guild.Id}");

			Assert.Empty(newGuild.Members);

			await gateway.OnGuildMemberAdd(member);

			user.Avatar = "new avi";

			await gateway.OnUserUpdate(user);

			var x = await client.HashValuesAsync<DiscordGuildMemberPacket>($"discord:guild:{guild.Id}:members");

			newGuild.Members.AddRange(x);
			DiscordUserPacket currentUser = await client.GetAsync<DiscordUserPacket>($"discord:user:{user.Id}");

			Assert.NotEmpty(newGuild.Members);

			DiscordGuildMemberPacket newUser = newGuild.Members[0];

			Assert.Equal("new avi", currentUser.Avatar);
		}

		[Fact]
		public async Task GuildMemberAsync()
		{
			ResetObjects();

			await gateway.OnGuildMemberRemove(guild.Id, user);

			DiscordGuildPacket g = await client.GetAsync<DiscordGuildPacket>($"discord:guild:{guild.Id}");

			Assert.Empty(g.Members);
			Assert.Equal(g.Members.Count, g.MemberCount);


			await gateway.OnGuildMemberAdd(member);

			 g = await client.GetAsync<DiscordGuildPacket>($"discord:guild:{guild.Id}");

			Assert.Equal(g.Members.Count, g.MemberCount);
			Assert.NotEmpty(g.Members);

			await gateway.OnGuildMemberUpdate(new Common.Events.GuildMemberUpdateEventArgs
			{
				GuildId = guild.Id,
				Nickname = "new nick",
				RoleIds = new ulong[] { },
				User = user
			});

			//g = await client.GetAsync<DiscordGuildMemberPacket>($"discord:members:{guild.Id}");

			Assert.NotEmpty(g.Members);
			Assert.Equal(g.Members.Count, g.MemberCount);
			Assert.Equal("new nick", g.Members[0].Nickname);
		}

		[Fact]
        public async Task ChannelAsync()
        {
			ResetObjects();

			Assert.True(channel.IsNsfw);
			Assert.Equal("test channel", channel.Name);

			channel.IsNsfw = false;
			channel.Name = "new test channel";

			await gateway.OnChannelUpdate(channel);

			DiscordChannelPacket[] channels = await client.HashValuesAsync<DiscordChannelPacket>($"discord:channels:{channel.Id}");
			Assert.NotNull(newChannel);

			DiscordGuildPacket newGuild = await client.GetAsync<DiscordGuildPacket>($"discord:guild:{guild.Id}");
			Assert.NotNull(newGuild);

			Assert.False(newChannel.IsNsfw);
			Assert.False(newGuild.Channels[0].IsNsfw);

			Assert.Equal("new test channel", newChannel.Name);
			Assert.Equal("new test channel", newGuild.Channels[0].Name);

			DiscordChannelPacket otherChannel = new DiscordChannelPacket
			{
				GuildId = guild.Id,
				Id = 451,
				Name = "another channel",
				Type = ChannelType.GUILDTEXT,
				IsNsfw = false,
				Topic = "lol this is a channel",
			};

			await gateway.OnChannelCreate(otherChannel);

			DiscordChannelPacket otherAddedChannel = await client.GetAsync<DiscordChannelPacket>($"discord:channel:{otherChannel.Id}");
			newGuild = await client.GetAsync<DiscordGuildPacket>($"discord:guild:{guild.Id}");

			Assert.False(newGuild.Channels.First(x => x.Id == 451).IsNsfw);

			Assert.Equal("another channel", otherAddedChannel.Name);
			Assert.Equal("another channel", newGuild.Channels.First(x => x.Id == 451).Name);

			Assert.NotNull(otherAddedChannel);
			Assert.Equal("another channel", otherAddedChannel.Name);
			Assert.Equal(ChannelType.GUILDTEXT, otherAddedChannel.Type);
			Assert.False(otherChannel.IsNsfw);
			Assert.Equal("lol this is a channel", otherAddedChannel.Topic);

			await gateway.OnChannelDelete(otherChannel);
			otherAddedChannel = await client.GetAsync<DiscordChannelPacket>($"discord:channel:{otherChannel.Id}");

			Assert.Null(otherAddedChannel);
		}

		[Fact]
		public async Task GuildAsync()
		{
			ResetObjects();

			await gateway.OnGuildDelete(new DiscordGuildUnavailablePacket
			{
				GuildId = guild.Id,
				IsUnavailable = true
			});

			var deletedGuild = await client.GetAsync<DiscordGuildPacket>($"discord:guild:{guild.Id}");
			Assert.Null(deletedGuild);

			await gateway.OnGuildCreate(guild);

			deletedGuild = await client.GetAsync<DiscordGuildPacket>($"discord:guild:{guild.Id}");
			Assert.NotNull(deletedGuild);

			await gateway.OnGuildDelete(new DiscordGuildUnavailablePacket
			{
				GuildId = guild.Id,
				IsUnavailable = true
			});

			deletedGuild = await client.GetAsync<DiscordGuildPacket>($"discord:guild:{guild.Id}");
			Assert.Null(deletedGuild);

			await gateway.OnGuildCreate(guild);

			deletedGuild = await client.GetAsync<DiscordGuildPacket>($"discord:guild:{guild.Id}");
			Assert.NotNull(deletedGuild);
		}

		[Fact]
		public async Task RoleAsync()
		{
			ResetObjects();

			await gateway.OnGuildRoleCreate(guild.Id, role);

			var updatedGuild = await client.GetAsync<DiscordGuildPacket>($"discord:guild:{guild.Id}");

			Assert.NotEmpty(updatedGuild.Roles); 
			Assert.Contains(updatedGuild.Roles, x => x.Id == role.Id);

			DiscordRolePacket updatedRole = updatedGuild.Roles.First();

			Assert.Equal(role.Name, updatedRole.Name);

			updatedRole.Name = "new role";
			updatedRole.Permissions = 123429;

			await gateway.OnGuildRoleUpdate(guild.Id, updatedRole);

			updatedGuild = await client.GetAsync<DiscordGuildPacket>($"discord:guild:{guild.Id}");

			Assert.NotEmpty(updatedGuild.Roles);
			Assert.Contains(updatedGuild.Roles, x => x.Id == role.Id);

			updatedRole = updatedGuild.Roles.First();

			Assert.Equal(123429, updatedRole.Permissions);
			Assert.Equal("new role", updatedRole.Name);

			await gateway.OnGuildRoleDelete(guild.Id, updatedRole.Id);

			updatedGuild = await client.GetAsync<DiscordGuildPacket>($"discord:guild:{guild.Id}");

			Assert.Empty(updatedGuild.Roles);
		}
	}
}
