using MessagePack;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.Serialization;

namespace Miki.Discord.Common.Packets
{
	[ProtoContract]
	[MessagePackObject]
	public class DiscordGuildMemberPacket
	{
        [DataMember(Name = "user")]
        [ProtoMember(1)]
		[Key(0)]
		public DiscordUserPacket User { get; set; }

		[ProtoMember(2)]
        [DataMember(Name = "guild_id")]
        [Key(1)]
		public ulong GuildId { get; set; }

		[ProtoMember(3)]
        [DataMember(Name = "nick")]
        [Key(2)]
		public string Nickname { get; set; }

		[ProtoMember(4)]
        [DataMember(Name = "roles")]
        [Key(3)]
		public List<ulong> Roles { get; set; } = new List<ulong>();

		[ProtoMember(5)]
		[Key(4)]
		public long JoinedAt { get; set; }

        [DataMember(Name = "joined_at")]
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
        [DataMember(Name = "deaf")]
        [Key(5)]
		public bool Deafened { get; set; }

		[ProtoMember(7)]
        [DataMember(Name = "mute")]
        [Key(6)]
		public bool Muted { get; set; }
	}
}