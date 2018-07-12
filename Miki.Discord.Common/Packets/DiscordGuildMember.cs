using Miki.Discord.Internal;
using Newtonsoft.Json;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Miki.Discord.Rest.Entities
{
	[ProtoContract]
	public class DiscordGuildMemberPacket
    {
		[JsonProperty("user")]
		public DiscordUserPacket User { get; set; }

		[ProtoMember(1)]
		public ulong UserId { get; set; }

		[ProtoMember(2)]
		[JsonProperty("guild_id")]
		public ulong GuildId { get; set; }

		[ProtoMember(3)]
		[JsonProperty("nick")]
		public string Nickname { get; set; }

		[ProtoMember(4)]
		[JsonProperty("roles")]
		public List<ulong> Roles { get; set; }

		[ProtoMember(5)]
		public long JoinedAt { get; set; }

		[JsonProperty("joined_at")]
		internal string _joinedAt
		{
			get
			{
				return new DateTime(JoinedAt).ToString("MM/dd/yyyy HH:mm:ss");
			}

			set
			{
				if (DateTime.TryParseExact(value, "MM/dd/yyyy HH:mm:ss", CultureInfo.CurrentCulture, DateTimeStyles.None, out DateTime d))
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
		public bool Deafened { get; set; }

		[ProtoMember(7)]
		[JsonProperty("mute")]
		public bool Muted { get; set; }
    }
}
