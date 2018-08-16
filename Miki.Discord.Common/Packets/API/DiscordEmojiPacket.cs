using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Miki.Discord.Common.Packets
{
	[ProtoContract]
    public class DiscordEmojiPacket
    {
		[ProtoMember(1)]
		[JsonProperty("id")]
		public ulong Id { get; set; }

		[ProtoMember(2)]
		[JsonProperty("name")]
		public string Name { get; set; }

		[ProtoMember(3)]
		[JsonProperty("roles")]
		public List<ulong> WhitelistedRoles { get; set; }

		[ProtoMember(4)]
		[JsonProperty("user")]
		public DiscordUserPacket Creator { get; set; }

		[ProtoMember(5)]
		[JsonProperty("require_colons")]
		public bool RequireColons { get; set; }

		[ProtoMember(6)]
		[JsonProperty("managed")]
		public bool Managed { get; set; }

		[ProtoMember(7)]
		[JsonProperty("animated")]
		public bool Animated { get; set; }
	}
}
