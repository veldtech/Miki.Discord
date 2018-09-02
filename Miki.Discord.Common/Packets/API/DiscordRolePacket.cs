using MessagePack;
using Newtonsoft.Json;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Text;

namespace Miki.Discord.Common.Packets
{
	[ProtoContract]
	[MessagePackObject]
    public class DiscordRolePacket
    {
		[ProtoMember(1)]
		[JsonProperty("id")]
		[Key(0)]
		public ulong Id;

		[ProtoMember(2)]
		[JsonProperty("name")]
		[Key(1)]
		public string Name;

		[ProtoMember(3)]
		[JsonProperty("color")]
		[Key(2)]
		public int Color;

		[ProtoMember(4)]
		[JsonProperty("hoist")]
		[Key(3)]
		public bool IsHoisted;

		[ProtoMember(5)]
		[JsonProperty("position")]
		[Key(4)]
		public int Position;

		[ProtoMember(6)]
		[JsonProperty("permissions")]
		[Key(5)]
		public int Permissions;

		[ProtoMember(7)]
		[JsonProperty("managed")]
		[Key(6)]
		public bool Managed;

		[ProtoMember(8)]
		[JsonProperty("mentionable")]
		[Key(7)]
		public bool Mentionable;
	}
}
