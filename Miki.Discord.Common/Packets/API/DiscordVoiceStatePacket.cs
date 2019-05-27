using ProtoBuf;
using System.Runtime.Serialization;

namespace Miki.Discord.Common.Packets
{
	[ProtoContract]
    [DataContract]
    public class DiscordVoiceStatePacket
	{
		[ProtoMember(1)]
		[DataMember(Name ="guild_id")]
		public ulong? GuildId { get; set; }

		[ProtoMember(2)]
		[DataMember(Name ="channel_id")]
		public ulong ChannelId { get; set; }

		[ProtoMember(3)]
		[DataMember(Name ="user_id")]
		public ulong UserId { get; set; }

		[ProtoMember(4)]
		[DataMember(Name ="session_id")]
		public string SessionId { get; set; }

		[ProtoMember(5)]
		[DataMember(Name ="deaf")]
		public bool Deafened { get; set; }

		[ProtoMember(6)]
		[DataMember(Name ="mute")]
		public bool Muted { get; set; }

		[ProtoMember(7)]
		[DataMember(Name ="self_deaf")]
		public bool SelfDeafened { get; set; }

		[ProtoMember(8)]
		[DataMember(Name ="self_mute")]
		public bool SelfMuted { get; set; }

		[ProtoMember(9)]
		[DataMember(Name ="suppress")]
		public bool Suppressed { get; set; }
	}
}