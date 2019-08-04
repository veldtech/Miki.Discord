using System.Runtime.Serialization;

namespace Miki.Discord.Common.Packets
{
	[DataContract]
	public class DiscordVoiceStatePacket
	{
		[DataMember(Name = "guild_id", Order = 1)]
		public ulong? GuildId { get; set; }

		[DataMember(Name = "channel_id", Order = 2)]
		public ulong ChannelId { get; set; }

		[DataMember(Name = "user_id", Order = 3)]
		public ulong UserId { get; set; }

		[DataMember(Name = "session_id", Order = 4)]
		public string SessionId { get; set; }

		[DataMember(Name = "deaf", Order = 5)]
		public bool Deafened { get; set; }

		[DataMember(Name = "mute", Order = 6)]
		public bool Muted { get; set; }

		[DataMember(Name = "self_deaf", Order = 7)]
		public bool SelfDeafened { get; set; }

		[DataMember(Name = "self_mute", Order = 8)]
		public bool SelfMuted { get; set; }

		[DataMember(Name = "suppress", Order = 9)]
		public bool Suppressed { get; set; }
	}
}