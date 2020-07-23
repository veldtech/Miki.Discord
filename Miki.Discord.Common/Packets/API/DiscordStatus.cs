using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Miki.Discord.Common
{
    [DataContract]
    public class DiscordStatus
    {
        [JsonPropertyName("since")]
        [DataMember(Name = "since", Order = 1)]
        public int? Since { get; set; }

        [JsonPropertyName("game")]
        [DataMember(Name = "game", Order = 2)]
        public DiscordActivity Game { get; set; }

        [JsonPropertyName("status")]
        [DataMember(Name = "status", Order = 3)]
        public string Status { get; set; }

        [JsonPropertyName("afk")]
        [DataMember(Name = "afk", Order = 4)]
        public bool IsAFK { get; set; }
    }
}