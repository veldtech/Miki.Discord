namespace Miki.Discord.Common.Events
{
    using System.Runtime.Serialization;
    using System.Text.Json.Serialization;

    [DataContract]
    public class DiscordMessageReactionRemoveEventArgs
    {
        [JsonPropertyName("user_id")]
        [DataMember(Name = "user_id", Order = 1)]
        public ulong UserId { get; set; }

        [JsonPropertyName("channel_id")]
        [DataMember(Name = "channel_id", Order = 2)]
        public ulong ChannelId { get; set; }

        [JsonPropertyName("message_id")]
        [DataMember(Name = "message_id", Order = 3)]
        public ulong MessageId { get; set; }

        [JsonPropertyName("guild_id")]
        [DataMember(Name = "guild_id", Order = 4)]
        public ulong? GuildId { get; set; }

        [JsonPropertyName("emoji")]
        [DataMember(Name = "emoji", Order = 5)]
        public DiscordEmoji Emoji { get; set; }
    }
}
