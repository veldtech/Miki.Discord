using MessagePack;
using Newtonsoft.Json;
using ProtoBuf;

namespace Miki.Discord.Common.Packets
{
	[ProtoContract]
	[MessagePackObject]
	public class DiscordVoiceStatePacket
	{
		[ProtoMember(1)]
		[JsonProperty("guild_id")]
		[Key(0)]
		public ulong? GuildId { get; set; }

		[ProtoMember(2)]
		[JsonProperty("channel_id")]
		[Key(1)]
		public ulong ChannelId { get; set; }

		[ProtoMember(3)]
		[JsonProperty("user_id")]
		[Key(2)]
		public ulong UserId { get; set; }

		[ProtoMember(4)]
		[JsonProperty("session_id")]
		[Key(3)]
		public string SessionId { get; set; }

		[ProtoMember(5)]
		[JsonProperty("deaf")]
		[Key(4)]
		public bool Deafened { get; set; }

		[ProtoMember(6)]
		[JsonProperty("mute")]
		[Key(5)]
		public bool Muted { get; set; }

		[ProtoMember(7)]
		[JsonProperty("self_deaf")]
		[Key(6)]
		public bool SelfDeafened { get; set; }

		[ProtoMember(8)]
		[JsonProperty("self_mute")]
		[Key(7)]
		public bool SelfMuted { get; set; }

		[ProtoMember(9)]
		[JsonProperty("suppress")]
		[Key(8)]
		public bool Suppressed { get; set; }
	}
}
 