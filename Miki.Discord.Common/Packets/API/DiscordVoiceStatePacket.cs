using MessagePack;
using ProtoBuf;
using System.Runtime.Serialization;

namespace Miki.Discord.Common.Packets
{
	[ProtoContract]
	[MessagePackObject]
    [DataContract]
    public class DiscordVoiceStatePacket
	{
		[ProtoMember(1)]
		[DataMember(Name ="guild_id")]
		[Key(0)]
		public ulong? GuildId { get; set; }

		[ProtoMember(2)]
		[DataMember(Name ="channel_id")]
		[Key(1)]
		public ulong ChannelId { get; set; }

		[ProtoMember(3)]
		[DataMember(Name ="user_id")]
		[Key(2)]
		public ulong UserId { get; set; }

		[ProtoMember(4)]
		[DataMember(Name ="session_id")]
		[Key(3)]
		public string SessionId { get; set; }

		[ProtoMember(5)]
		[DataMember(Name ="deaf")]
		[Key(4)]
		public bool Deafened { get; set; }

		[ProtoMember(6)]
		[DataMember(Name ="mute")]
		[Key(5)]
		public bool Muted { get; set; }

		[ProtoMember(7)]
		[DataMember(Name ="self_deaf")]
		[Key(6)]
		public bool SelfDeafened { get; set; }

		[ProtoMember(8)]
		[DataMember(Name ="self_mute")]
		[Key(7)]
		public bool SelfMuted { get; set; }

		[ProtoMember(9)]
		[DataMember(Name ="suppress")]
		[Key(8)]
		public bool Suppressed { get; set; }
	}
}