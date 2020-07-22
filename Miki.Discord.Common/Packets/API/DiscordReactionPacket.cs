using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Miki.Discord.Common
{
    [DataContract]
    public class DiscordReactionCountPacket
    {
        [JsonPropertyName("count")]
        [DataMember(Name = "count")]
        public int Count { get; set; }

        [JsonPropertyName("me")]
        [DataMember(Name = "me")]
        public bool Me { get; set; }

        [JsonPropertyName("emoji")]
        [DataMember(Name = "emoji")]
        public DiscordEmoji Emoji { get; set; }
    }
}