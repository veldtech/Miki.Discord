using Newtonsoft.Json;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Text;

namespace Miki.Discord.Rest.Entities
{
	[ProtoContract]
    public class DiscordGuildMember
    {
		[JsonProperty("user")]
		public DiscordUser User { get; set; }

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
		internal DateTimeOffset _joinedAt { get => DateTimeOffset.FromUnixTimeSeconds(JoinedAt); set => JoinedAt = value.ToUnixTimeSeconds(); }

		[ProtoMember(6)]
		[JsonProperty("deaf")]
		public bool Deafened { get; set; }

		[ProtoMember(7)]
		[JsonProperty("mute")]
		public bool Muted { get; set; }
    }
}
