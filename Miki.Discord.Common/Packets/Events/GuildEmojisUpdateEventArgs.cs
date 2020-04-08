namespace Miki.Discord.Common.Events
{
    using System.Runtime.Serialization;
    using System.Text.Json.Serialization;

    [DataContract]
    public class GuildEmojisUpdateEventArgs
    {
        [JsonPropertyName("guild_id")]
        [DataMember(Name = "guild_id")]
        public ulong guildId { get; set; }

        [JsonPropertyName("emojis")]
        [DataMember(Name = "emojis")]
        public DiscordEmoji[] emojis { get; set; }
    }
}