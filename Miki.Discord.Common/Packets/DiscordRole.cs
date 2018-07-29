using Newtonsoft.Json;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Text;

namespace Miki.Discord.Rest.Entities
{
	[ProtoContract]
    public class DiscordRolePacket
    {
		[ProtoMember(1)]
		[JsonProperty("id")]
		public ulong Id;

		[ProtoMember(2)]
		[JsonProperty("name")]
		public string Name;

		[ProtoMember(3)]
		[JsonProperty("color")]
		public int Color;

		[ProtoMember(4)]
		[JsonProperty("hoist")]
		public bool IsHoisted;

		[ProtoMember(5)]
		[JsonProperty("position")]
		public int Position;

		[ProtoMember(6)]
		[JsonProperty("permissions")]
		public int Permissions;

		[ProtoMember(7)]
		[JsonProperty("managed")]
		public bool Managed;

		[ProtoMember(8)]
		[JsonProperty("mentionable")]
		public bool Mentionable;
	}
}
