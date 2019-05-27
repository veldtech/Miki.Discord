using ProtoBuf;
using System.Runtime.Serialization;

namespace Miki.Discord.Common.Packets
{
    [DataContract]
    public class DiscordRolePacket
	{
		[DataMember(Name = "id", Order = 1)]
		public ulong Id;

		[DataMember(Name = "name", Order = 2)]
		public string Name;

		[DataMember(Name = "color", Order = 3)]
		public int Color;

		[DataMember(Name = "hoist", Order = 4)]
		public bool IsHoisted;

		[DataMember(Name = "position", Order = 5)]
		public int Position;

		[DataMember(Name = "permissions", Order = 6)]
		public int Permissions;

		[DataMember(Name = "managed", Order = 7)]
		public bool Managed;

		[DataMember(Name = "mentionable", Order = 8)]
		public bool Mentionable;
	}
}