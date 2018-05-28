using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Text;

namespace Miki.Discord.Rest.Entities
{
	[ProtoContract]
    public class DiscordEmoji
    {
		[ProtoMember(1)]
		public ulong Id;

		[ProtoMember(2)]
		public string Name;

		[ProtoMember(3)]
		public List<ulong> WhitelistedRoles;

		[ProtoMember(4)]
		public DiscordUser Creator;

		[ProtoMember(5)]
		public bool RequireColons;

		[ProtoMember(6)]
		public bool Managed;
    }
}
