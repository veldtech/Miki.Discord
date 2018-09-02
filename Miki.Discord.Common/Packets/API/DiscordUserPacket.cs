using MessagePack;
using Miki.Discord.Common;
using Newtonsoft.Json;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Miki.Discord.Common.Packets
{
	[ProtoContract]
	[MessagePackObject]
	public class DiscordUserPacket
	{
		[JsonProperty("id")]
		[ProtoMember(1)]
		[Key(0)]
		public ulong Id { get; set; }

		[JsonProperty("username")]
		[ProtoMember(2)]
		[Key(1)]
		public string Username { get; set; }

		[JsonProperty("discriminator")]
		[ProtoMember(3)]
		[Key(2)]
		public string Discriminator { get; set; }

		[JsonProperty("bot")]
		[ProtoMember(4)]
		[Key(3)]
		public bool IsBot { get; set; }

		[JsonProperty("avatar")]
		[ProtoMember(5)]
		[Key(4)]
		public string Avatar { get; set; }

		[JsonProperty("verified")]
		[ProtoMember(6)]
		[Key(5)]
		public bool Verified { get; set; }

		[JsonProperty("email")]
		[ProtoMember(7)]
		[Key(6)]
		public string Email { get; set; }

		[JsonProperty("mfa_enabled")]
		[ProtoMember(8)]
		[Key(7)]
		public bool MfaEnabled { get; set; }
	}
}