using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Miki.Discord.Common.Packets;

namespace Miki.Discord.Common
{
    [DataContract]
    public class DiscordActivity
    {
        [JsonPropertyName("name")]
        [DataMember(Name = "name", Order = 1)]
        public string Name { get; set; }

        [JsonPropertyName("type")]
        [DataMember(Name = "type", Order = 2)]
        public ActivityType Type { get; set; }

        [JsonPropertyName("url")]
        [DataMember(Name = "url", Order = 3)]
        public string Url { get; set; }

        [JsonPropertyName("timestamps")]
        [DataMember(Name = "timestamps", Order = 4)]
        public TimeStampsObject Timestamps { get; set; }

        [JsonPropertyName("application_id")]
        [DataMember(Name = "application_id", Order = 5)]
        public ulong? ApplicationId { get; set; }

        [JsonPropertyName("state")]
        [DataMember(Name = "state", Order = 6)]
        public string State { get; set; }

        [JsonPropertyName("details")]
        [DataMember(Name = "details", Order = 7)]
        public string Details { get; set; }

        [JsonPropertyName("party")]
        [DataMember(Name = "party", Order = 8)]
        public RichPresenceParty Party { get; set; }

        [JsonPropertyName("assets")]
        [DataMember(Name = "assets", Order = 9)]
        public RichPresenceAssets Assets { get; set; }
    }
}