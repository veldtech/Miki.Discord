using BenchmarkDotNet.Attributes;
using Miki.Cache;
using Miki.Cache.StackExchange;
using Miki.Discord.Common;
using Miki.Discord.Common.Packets;
using Miki.Discord.Mocking;
using Miki.Discord.Tests.Dummy;
using Miki.Serialization.MsgPack;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Miki.Discord.Tests
{
    [CoreJob]
    [RPlotExporter, RankColumn]
    public class CachePerformance
    {
        private DiscordGuildPacket packet;
        private StackExchangeCachePool pool;
        private DummyGateway gateway;
        private IExtendedCacheClient client;

        DiscordChannelPacket channel;
        DiscordGuildMemberPacket member;
        DiscordUserPacket user;
        DiscordRolePacket role;
        DiscordClient discordClient;
        Common.Events.GuildMemberUpdateEventArgs updateMember;

        [Params(1000, 10000)]
        public int N;

        [Params(16, 256, 2048, 10000)]
        public int MemberCount;

        [GlobalSetup]
        public async Task Setup()
        {
            packet = new DiscordGuildPacket
            {
                AfkChannelId = 245245,
                AfkTimeout = 13,
                ApplicationId = null,
                CreatedAt = 422424,
                IsOwner = true,
                IsLarge = false,
                MemberCount = 420,
                DefaultMessageNotifications = 0,
                MFALevel = 1,
                EmbedChannelId = null,
                EmbedEnabled = false,
                Emojis = new DiscordEmoji[]
                    { new DiscordEmoji(), new DiscordEmoji(), new DiscordEmoji() },
                Channels = new List<DiscordChannelPacket>(),
                Members = new List<DiscordGuildMemberPacket>(),
                ExplicitContentFilter = 2,
                Icon = "meme",
                Id = 34534534,
                Unavailable = false,
                Name = "WEJFWIEJF"
            };

            var members = new DiscordGuildMemberPacket[MemberCount];

            for(int i = 0; i < MemberCount; i++)
            {
                members[i] = new DiscordGuildMemberPacket();
                members[i].User = new DiscordUserPacket();
                members[i].User.Id = (ulong)i;
            }

            var channels = new DiscordChannelPacket[24];

            for(int i = 0; i < 24; i++)
            {
                channels[i] = new DiscordChannelPacket();
                channels[i].Id = (ulong)i;
            }

            packet.Channels.AddRange(channels);
            packet.Members.AddRange(members);

            pool = new StackExchangeCachePool(new LZ4MsgPackSerializer(), "localhost");
            gateway = new DummyGateway();

            discordClient = new DiscordClient(new DiscordClientConfigurations
            {
                ApiClient = new DefaultDummyApiClient(),
                Gateway = gateway,
                CacheClient = client
            });

            client = (IExtendedCacheClient)await pool.GetAsync();

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
                GuildId = 34534534,
                Id = 111,
                IsNsfw = true,
                Name = "test channel",
                Type = ChannelType.GUILDTEXT,
                CreatedAt = 0
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
                GuildId = 34534534,
                JoinedAt = 0,
                Deafened = false,
                Muted = false,
                Nickname = null,
                Roles = new List<ulong>(),
                User = user
            };

            updateMember = new Common.Events.GuildMemberUpdateEventArgs
            {
                GuildId = packet.Id,
                Nickname = "wewe",
                RoleIds = new ulong[0],
                User = user
            };
        }

        [Benchmark]
        public async Task AllPackets()
        {
            await gateway.OnGuildCreate(packet);
            await gateway.OnChannelCreate(channel);
            await gateway.OnChannelUpdate(channel);
            await gateway.OnChannelDelete(channel);
            await gateway.OnGuildMemberAdd(member);
            await gateway.OnGuildMemberUpdate(updateMember);
            await gateway.OnGuildMemberRemove(packet.Id, user);
            await gateway.OnGuildRoleCreate(packet.Id, role);
            await gateway.OnGuildRoleUpdate(packet.Id, role);
            await gateway.OnGuildRoleDelete(packet.Id, role.Id);
            await gateway.OnUserUpdate(new DiscordPresencePacket()
            {
                User = user,
                GuildId = packet.Id
            });
            await gateway.OnGuildUpdate(packet);
            await gateway.OnGuildDelete(new DiscordGuildUnavailablePacket { GuildId = packet.Id, IsUnavailable = true });
        }
    }
}
