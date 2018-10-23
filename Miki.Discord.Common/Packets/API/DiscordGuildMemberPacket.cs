using MessagePack;
using Miki.Discord.Common.Packets;
using Newtonsoft.Json;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Miki.Discord.Common.Packets
{
	[ProtoContract]
	[MessagePackObject]
	public class DiscordGuildMemberPacket
    {
		[JsonProperty("user")]
		[ProtoMember(1)]
		[Key(0)]
		public DiscordUserPacket User { get; set; }

		[ProtoMember(2)]
		[JsonProperty("guild_id")]
		[Key(1)]
		public ulong GuildId { get; set; }

		[ProtoMember(3)]
		[JsonProperty("nick")]
		[Key(2)]
		public string Nickname { get; set; }

		[ProtoMember(4)]
		[JsonProperty("roles")]
		[Key(3)]
		public List<ulong> Roles { get; set; } = new List<ulong>();

		[ProtoMember(5)]
		[Key(4)]
		public long JoinedAt { get; set; }

		[JsonProperty("joined_at")]
		internal string _joinedAt
		{
			get
			{
				return new DateTime(JoinedAt).ToString("MM-dd-yyyyTHH:mm:ss.fffffffzzz");
			}

			set
			{
				if (DateTime.TryParseExact(value, "MM-dd-yyyyTHH:mm:ss.fffffffzzz", CultureInfo.CurrentCulture, DateTimeStyles.None, out DateTime d))
				{
					JoinedAt = d.Ticks;
				}

				if (DateTime.TryParse(value, out DateTime e))
				{
					JoinedAt = e.Ticks;
				}
			}
		}

		[ProtoMember(6)]
		[JsonProperty("deaf")]
		[Key(5)]
		public bool Deafened { get; set; }

		[ProtoMember(7)]
		[JsonProperty("mute")]
		[Key(6)]
		public bool Muted { get; set; }
    }
}
