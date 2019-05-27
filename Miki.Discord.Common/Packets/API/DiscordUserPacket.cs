using ProtoBuf;
using System.Runtime.Serialization;

namespace Miki.Discord.Common.Packets
{
    [DataContract]
    public class DiscordUserPacket
	{
		[DataMember(Name ="id", Order = 1)]
		public ulong Id { get; set; }

		[DataMember(Name = "username", Order = 2)]
		public string Username { get; set; }

		[DataMember(Name = "discriminator", Order = 3)]
		public string Discriminator { get; set; }

		[DataMember(Name = "bot", Order = 4)]
		public bool IsBot { get; set; }

		[DataMember(Name = "avatar", Order = 5)]
		public string Avatar { get; set; }

		[DataMember(Name = "verified", Order = 6)]
		public bool Verified { get; set; }

		[DataMember(Name = "email", Order = 7)]
		public string Email { get; set; }

		[DataMember(Name ="mfa_enabled", Order = 8)]
		public bool MfaEnabled { get; set; }
	}
}