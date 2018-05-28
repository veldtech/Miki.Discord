using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Text;

namespace Miki.Discord.Rest.Entities
{
	[ProtoContract]
    public class DiscordRole
    {
		[ProtoMember(1)]
		public ulong Id;

		[ProtoMember(2)]
		public string Name;

		[ProtoMember(3)]
		public int Color;

		[ProtoMember(4)]
		public bool IsHoisted;

		[ProtoMember(5)]
		public int Position;

		[ProtoMember(6)]
		public int Permissions;

		[ProtoMember(7)]
		public bool Managed;

		[ProtoMember(8)]
		public bool Mentionable;
    }
}
