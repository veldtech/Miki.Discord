using Newtonsoft.Json;
using ProtoBuf;

namespace Miki.Discord.Common.Packets
{
	[ProtoContract]
	public class DiscordVoiceStatePacket
	{
		[ProtoMember(1)]
		[JsonProperty("guild_id")]
		public ulong? GuildId { get; set; }

		[ProtoMember(2)]
		[JsonProperty("channel_id")]
		public ulong ChannelId { get; set; }

		[ProtoMember(3)]
		[JsonProperty("user_id")]
		public ulong UserId { get; set; }

		[ProtoMember(4)]
		[JsonProperty("session_id")]
		public string SessionId { get; set; }

		[ProtoMember(5)]
		[JsonProperty("deaf")]
		public bool Deafened { get; set; }

		[ProtoMember(6)]
		[JsonProperty("mute")]
		public bool Muted { get; set; }

		[ProtoMember(7)]
		[JsonProperty("self_deaf")]
		public bool SelfDeafened { get; set; }

		[ProtoMember(8)]
		[JsonProperty("self_mute")]
		public bool SelfMuted { get; set; }

		[ProtoMember(9)]
		[JsonProperty("suppress")]
		public bool Suppressed { get; set; }
	}
}
 