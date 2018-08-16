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
	public class DiscordUserPacket
	{
		[JsonProperty("id")]
		[ProtoMember(1)]
		public ulong Id { get; set; }

		[JsonProperty("username")]
		[ProtoMember(2)]
		public string Username { get; set; }

		[JsonProperty("discriminator")]
		[ProtoMember(3)]
		public string Discriminator { get; set; }

		[JsonProperty("bot")]
		[ProtoMember(4)]
		public bool IsBot { get; set; }

		[JsonProperty("avatar")]
		[ProtoMember(5)]
		public string Avatar { get; set; }

		[JsonProperty("verified")]
		[ProtoMember(6)]
		public bool Verified { get; set; }

		[JsonProperty("email")]
		[ProtoMember(7)]
		public string Email { get; set; }

		[JsonProperty("mfa_enabled")]
		[ProtoMember(8)]
		public bool MfaEnabled { get; set; }
	}
}