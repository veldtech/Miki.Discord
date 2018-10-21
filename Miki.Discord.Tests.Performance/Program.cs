using BenchmarkDotNet.Running;
using Miki.Discord.Common;
using Miki.Discord.Common.Packets;
using Miki.Serialization.MsgPack;
using Miki.Serialization.Protobuf;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Miki.Discord.Tests.Performance
{
	class Program
	{
		static void Main(string[] args)
		{
			var p = new DiscordGuildPacket
			{
				AfkChannelId = 245245,
				AfkTimeout = 13,
				ApplicationId = null,
				CreatedAt = 422424,
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

			for (int i = 0; i < p.MemberCount; i++)
			{
				members[i] = new DiscordGuildMemberPacket();
				members[i].User = new DiscordUserPacket();
				members[i].User.Avatar = "iwjefpowjef";
				members[i].User.Id = (ulong)i;
				members[i].Roles = new List<ulong>() { 234234, 23424234234, 23435234, 345342423 };
				members[i].JoinedAt = 23984923;
				members[i].User.Discriminator = "1234";
				members[i].User.Id = (ulong)i;
			}

			var channels = new DiscordChannelPacket[24];

			for (int i = 0; i < 24; i++)
			{
				channels[i] = new DiscordChannelPacket();
				channels[i].Id = (ulong)i;
			}

			p.Channels.AddRange(channels);
			p.Members.AddRange(members);

			var pbuf = new ProtobufSerializer();

			var msgp = new MsgPackSerializer();

			var sw = Stopwatch.StartNew();

			for (int i = 0; i < 100000; i++)
			{
				pbuf.Serialize(p);
			}

			sw.Stop();

			Console.WriteLine("PBUF " + pbuf.Serialize(p).Length);
			Console.WriteLine("T 100K: " + ((double)sw.ElapsedTicks / Stopwatch.Frequency));

			sw.Restart();

			for (int i = 0; i < 100000; i++)
			{
				msgp.Serialize(p);
			}

			sw.Stop();

			Console.WriteLine("MSGP " + msgp.Serialize(p).Length);
			Console.WriteLine("T 100K: " + ((double)sw.ElapsedTicks / Stopwatch.Frequency));

			Console.ReadLine();

			var summary = BenchmarkRunner.Run<CachePerformance>();
			Console.ReadLine();
		}
	}
}
