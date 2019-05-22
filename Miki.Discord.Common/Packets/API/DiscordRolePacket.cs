using MessagePack;
using ProtoBuf;
using System.Runtime.Serialization;

namespace Miki.Discord.Common.Packets
{
	[ProtoContract]
	[MessagePackObject]
    [DataContract]
    public class DiscordRolePacket
	{
		[ProtoMember(1)]
		[DataMember(Name ="id")]
		[Key(0)]
		public ulong Id;

		[ProtoMember(2)]
		[DataMember(Name ="name")]
		[Key(1)]
		public string Name;

		[ProtoMember(3)]
		[DataMember(Name ="color")]
		[Key(2)]
		public int Color;

		[ProtoMember(4)]
		[DataMember(Name ="hoist")]
		[Key(3)]
		public bool IsHoisted;

		[ProtoMember(5)]
		[DataMember(Name ="position")]
		[Key(4)]
		public int Position;

		[ProtoMember(6)]
		[DataMember(Name ="permissions")]
		[Key(5)]
		public int Permissions;

		[ProtoMember(7)]
		[DataMember(Name ="managed")]
		[Key(6)]
		public bool Managed;

		[ProtoMember(8)]
		[DataMember(Name ="mentionable")]
		[Key(7)]
		public bool Mentionable;
	}
}