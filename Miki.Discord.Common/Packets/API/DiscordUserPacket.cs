using MessagePack;
using ProtoBuf;
using System.Runtime.Serialization;

namespace Miki.Discord.Common.Packets
{
	[ProtoContract]
	[MessagePackObject]
    [DataContract]
    public class DiscordUserPacket
	{
		[DataMember(Name ="id")]
		[ProtoMember(1)]
		[Key(0)]
		public ulong Id { get; set; }

		[DataMember(Name ="username")]
		[ProtoMember(2)]
		[Key(1)]
		public string Username { get; set; }

		[DataMember(Name ="discriminator")]
		[ProtoMember(3)]
		[Key(2)]
		public string Discriminator { get; set; }

		[DataMember(Name ="bot")]
		[ProtoMember(4)]
		[Key(3)]
		public bool IsBot { get; set; }

		[DataMember(Name ="avatar")]
		[ProtoMember(5)]
		[Key(4)]
		public string Avatar { get; set; }

		[DataMember(Name ="verified")]
		[ProtoMember(6)]
		[Key(5)]
		public bool Verified { get; set; }

		[DataMember(Name ="email")]
		[ProtoMember(7)]
		[Key(6)]
		public string Email { get; set; }

		[DataMember(Name ="mfa_enabled")]
		[ProtoMember(8)]
		[Key(7)]
		public bool MfaEnabled { get; set; }
	}
}