namespace Miki.Discord.Common
{
    using System.Runtime.Serialization;
    using System.Text.Json.Serialization;

    [DataContract]
    public class DiscordVoiceStatePacket
    {
        [JsonPropertyName("guild_id")]
        [DataMember(Name = "guild_id", Order = 1)]
        public ulong? GuildId { get; set; }

        [JsonPropertyName("channel_id")]
        [DataMember(Name = "channel_id", Order = 2)]
        public ulong ChannelId { get; set; }

        [JsonPropertyName("user_id")]
        [DataMember(Name = "user_id", Order = 3)]
        public ulong UserId { get; set; }

        [JsonPropertyName("session_id")]
        [DataMember(Name = "session_id", Order = 4)]
        public string SessionId { get; set; }

        [JsonPropertyName("deaf")]
        [DataMember(Name = "deaf", Order = 5)]
        public bool Deafened { get; set; }

        [JsonPropertyName("mute")]
        [DataMember(Name = "mute", Order = 6)]
        public bool Muted { get; set; }

        [JsonPropertyName("self_deaf")]
        [DataMember(Name = "self_deaf", Order = 7)]
        public bool SelfDeafened { get; set; }

        [JsonPropertyName("self_mute")]
        [DataMember(Name = "self_mute", Order = 8)]
        public bool SelfMuted { get; set; }

        [JsonPropertyName("suppress")]
        [DataMember(Name = "suppress", Order = 9)]
        public bool Suppressed { get; set; }
    }
}