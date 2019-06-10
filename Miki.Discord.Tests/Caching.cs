using Miki.Cache;
using Miki.Cache.InMemory;
using Miki.Discord.Common;
using Miki.Discord.Common.Packets;
using Miki.Discord.Tests.Dummy;
using Miki.Serialization.Protobuf;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Miki.Discord.Mocking;
using Xunit;
using System;
using System.Globalization;

namespace Miki.Discord.Tests
{
	/// <summary>
	/// Contains tests for Caching
	/// </summary>
    public class Caching
    {
		DummyGateway gateway;
		ICachePool pool;
		IExtendedCacheClient client;

		DiscordChannelPacket channel;
		DiscordGuildPacket guild;
		DiscordGuildMemberPacket member;
		DiscordUserPacket user;
		DiscordRolePacket role;
        DiscordClient discordClient;

		private async Task ResetObjectsAsync()
        {
            gateway = new DummyGateway();
            pool = new InMemoryCachePool(new ProtobufSerializer());
			client = (IExtendedCacheClient) await pool.GetAsync();
            discordClient = new DiscordClient(new DiscordClientConfigurations
            {
                ApiClient = new InvalidDummyApiClient(),
                Gateway = gateway,
                CacheClient = client
            });

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
                Emojis = new[]
                {
                    new DiscordEmoji() 
                }
            };
		}

		[Fact]
		public async Task UserUpdateAsync()
		{
			await ResetObjectsAsync();
			await gateway.OnGuildMemberRemove(guild.Id, member.User);

			var m = await client.HashGetAsync<DiscordGuildMemberPacket>(CacheUtils.GuildMembersKey(guild.Id), member.User.Id.ToString());
			
			await gateway.OnGuildMemberAdd(member);

			user.Avatar = "new avi";

			await gateway.OnUserUpdate(new DiscordPresencePacket()
			{
				User = user,
				GuildId = guild.Id
			});

			var x = await client.HashValuesAsync<DiscordGuildMemberPacket>(CacheUtils.GuildMembersKey(guild.Id));

			DiscordUserPacket currentUser = await client.HashGetAsync<DiscordUserPacket>(CacheUtils.UsersCacheKey, user.Id.ToString());

			Assert.NotEmpty(x);
			Assert.Equal("new avi", currentUser.Avatar);
		}	

		[Fact]
		public async Task GuildMemberAsync()
        {
            await ResetObjectsAsync();
            await gateway.OnGuildMemberRemove(guild.Id, user);

			DiscordGuildMemberPacket[] g = (await client.HashValuesAsync<DiscordGuildMemberPacket>(CacheUtils.GuildMembersKey(guild.Id))).ToArray();

			Assert.Empty(g);

			await gateway.OnGuildMemberAdd(member);

			 g = (await client.HashValuesAsync<DiscordGuildMemberPacket>(CacheUtils.GuildMembersKey(guild.Id))).ToArray();

			Assert.NotEmpty(g);

			await gateway.OnGuildMemberUpdate(new Common.Events.GuildMemberUpdateEventArgs
			{
				GuildId = guild.Id,
				Nickname = "new nick",
				RoleIds = new ulong[] { },
				User = user
			});

			g = (await client.HashValuesAsync<DiscordGuildMemberPacket>(CacheUtils.GuildMembersKey(guild.Id))).ToArray();

			Assert.NotEmpty(g);
			Assert.Equal("new nick", g[0].Nickname);
		}

		[Fact]
        public async Task ChannelAsync()
        {
            await ResetObjectsAsync();

            Assert.True(channel.IsNsfw);
			Assert.Equal("test channel", channel.Name);

			channel.IsNsfw = false;
			channel.Name = "new test channel";

			await gateway.OnChannelUpdate(channel);

			DiscordChannelPacket[] channels = (await client.HashValuesAsync<DiscordChannelPacket>(CacheUtils.ChannelsKey(guild.Id))).ToArray();
		
			Assert.NotEmpty(channels);

			var newChannel = channels.First();

			Assert.NotNull(newChannel);
			
			Assert.False(newChannel.IsNsfw);
			
			Assert.Equal("new test channel", newChannel.Name);

			DiscordChannelPacket otherChannel = new DiscordChannelPacket
			{
				GuildId = guild.Id,
				Id = 451,
				Name = "another channel",
				Type = ChannelType.GUILDTEXT,
				IsNsfw = false,
				Topic = "lol this is a channel"
            };

			await gateway.OnChannelCreate(otherChannel);

			DiscordChannelPacket otherAddedChannel = await client.HashGetAsync<DiscordChannelPacket>(
				CacheUtils.ChannelsKey(guild.Id), otherChannel.Id.ToString());

			Assert.Equal("another channel", otherAddedChannel.Name);

			Assert.NotNull(otherAddedChannel);
			Assert.Equal("another channel", otherAddedChannel.Name);
			Assert.Equal(ChannelType.GUILDTEXT, otherAddedChannel.Type);
			Assert.False(otherChannel.IsNsfw);
			Assert.Equal("lol this is a channel", otherAddedChannel.Topic);

			await gateway.OnChannelDelete(otherChannel);
			otherAddedChannel = await client.HashGetAsync<DiscordChannelPacket>(
				CacheUtils.ChannelsKey(guild.Id), otherAddedChannel.Id.ToString());

			Assert.Null(otherAddedChannel);
		}

		[Fact]
		public async Task GuildAsync()
		{
            await ResetObjectsAsync();

            await gateway.OnGuildDelete(new DiscordGuildUnavailablePacket
			{
				GuildId = guild.Id,
				IsUnavailable = true
			});

			var deletedGuild = await client.HashGetAsync<DiscordGuildPacket>(CacheUtils.GuildsCacheKey, guild.Id.ToString());
			Assert.Null(deletedGuild);

			await gateway.OnGuildCreate(guild);

			deletedGuild = await client.HashGetAsync<DiscordGuildPacket>(
				CacheUtils.GuildsCacheKey, guild.Id.ToString()
			);

			Assert.NotNull(deletedGuild);

			await gateway.OnGuildEmojiUpdate(deletedGuild.Id, new DiscordEmoji[] { new DiscordEmoji() });

			deletedGuild = await client.HashGetAsync<DiscordGuildPacket>(
				CacheUtils.GuildsCacheKey, guild.Id.ToString()
			);

			Assert.NotEmpty(deletedGuild.Emojis);

			await gateway.OnGuildDelete(new DiscordGuildUnavailablePacket
			{
				GuildId = guild.Id,
				IsUnavailable = true
			});

			deletedGuild = await client.HashGetAsync<DiscordGuildPacket>(
				CacheUtils.GuildsCacheKey, guild.Id.ToString()
			);
			Assert.Null(deletedGuild);

			await gateway.OnGuildCreate(guild);

			deletedGuild = await client.HashGetAsync<DiscordGuildPacket>(
				CacheUtils.GuildsCacheKey, guild.Id.ToString()
			);
			Assert.NotNull(deletedGuild);
		}

		[Fact]
		public async Task RoleAsync()
        {
            await ResetObjectsAsync();

            await gateway.OnGuildRoleCreate(guild.Id, role);

			var updatedRole = await client.HashGetAsync<DiscordRolePacket>(CacheUtils.GuildRolesKey(guild.Id), role.Id.ToString());

			Assert.NotNull(updatedRole); 

			Assert.Equal(role.Name, updatedRole.Name);

			updatedRole.Name = "new role";
			updatedRole.Permissions = 123429;

			await gateway.OnGuildRoleUpdate(guild.Id, updatedRole);

			updatedRole = await client.HashGetAsync<DiscordRolePacket>(CacheUtils.GuildRolesKey(guild.Id), role.Id.ToString());

			Assert.NotNull(updatedRole);

			Assert.Equal(123429, updatedRole.Permissions);
			Assert.Equal("new role", updatedRole.Name);

			await gateway.OnGuildRoleDelete(guild.Id, updatedRole.Id);

			updatedRole = await client.HashGetAsync<DiscordRolePacket>(CacheUtils.GuildRolesKey(guild.Id), role.Id.ToString());

			Assert.Null(updatedRole);
		}

        [Fact]
        public async Task GuildOperations()
        {
            await ResetObjectsAsync();

            await gateway.OnGuildDelete(new DiscordGuildUnavailablePacket
            {
                GuildId = guild.Id,
                IsUnavailable = false
            });

            await Assert.ThrowsAsync<InvalidOperationException>(
                async () => await discordClient.GetGuildAsync(guild.Id));

            await gateway.OnGuildCreate(guild);

            var guildObject = await discordClient.GetGuildAsync(guild.Id);

            Assert.Equal(guild.Id, guildObject.Id);
            Assert.Equal(DiscordUtils.GetAvatarUrl(guild.Id, guild.Icon), guildObject.IconUrl);
            Assert.Equal(guild.MemberCount, guildObject.MemberCount);
        }

        [Fact]
        public async Task CultureChanged()
        {
            await ResetObjectsAsync();

            var now = DateTime.ParseExact("01/01/2019 00:00:00", "MM/dd/yyyy HH:mm:ss", CultureInfo.InvariantCulture).Ticks;

            CultureInfo.CurrentCulture = new CultureInfo("nl-NL");

            var guildPacket = new DiscordGuildPacket
            {
                CreatedAt = now
            };

            await client.UpsertAsync("guild", guildPacket);

            CultureInfo.CurrentCulture = new CultureInfo("en-US");

            var cachedGuild = await client.GetAsync<DiscordGuildPacket>("guild");

            Assert.Equal(now, cachedGuild.CreatedAt);
        }
    }
}
