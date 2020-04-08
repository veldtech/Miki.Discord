namespace Miki.Discord.Tests.Performance
{
    using BenchmarkDotNet.Running;
    using Miki.Discord.Common;
    using Miki.Discord.Common.Packets;
    using Miki.Serialization.MsgPack;
    using Miki.Serialization.Protobuf;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;

    internal class Program
    {
        private static void Main()
        {
            var p = new DiscordGuildPacket
            {
                AfkChannelId = 245245,
                AfkTimeout = 13,
                ApplicationId = null,
                CreatedAt = new DateTime(422424),
                IsOwner = true,
                IsLarge = false,
                MemberCount = 10000,
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

            var members = new DiscordGuildMemberPacket[p.MemberCount.Value];

            for(int i = 0; i < p.MemberCount; i++)
            {
                members[i] = new DiscordGuildMemberPacket
                {
                    User = new DiscordUserPacket
                    {
                        Avatar = "iwjefpowjef",
                        Id = (ulong) i,
                        Discriminator = "1234",

                    },
                    Roles = new List<ulong>()
                    {
                        234234,
                        23424234234,
                        23435234,
                        345342423
                    },
                    JoinedAt = new DateTime(314900)
                };
            }

            var channels = new DiscordChannelPacket[24];

            for(int i = 0; i < 24; i++)
            {
                channels[i] = new DiscordChannelPacket
                {
                    Id = (ulong) i
                };
            }

            p.Channels.AddRange(channels);
            p.Members.AddRange(members);

            var pbuf = new ProtobufSerializer();

            var msgp = new MsgPackSerializer();

            var sw = Stopwatch.StartNew();

            for(int i = 0; i < 100000; i++)
            {
                pbuf.Serialize(p);
            }

            sw.Stop();

            Console.WriteLine("PBUF " + pbuf.Serialize(p).Length);
            Console.WriteLine("T 100K: " + ((double)sw.ElapsedTicks / Stopwatch.Frequency));

            sw.Restart();

            for(int i = 0; i < 100000; i++)
            {
                msgp.Serialize(p);
            }

            sw.Stop();

            Console.WriteLine("MSGP " + msgp.Serialize(p).Length);
            Console.WriteLine("T 100K: " + ((double)sw.ElapsedTicks / Stopwatch.Frequency));

            Console.ReadLine();

            BenchmarkRunner.Run<CachePerformance>();
            Console.ReadLine();
        }
    }
}
