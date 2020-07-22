using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Miki.Discord.Common
{
    [DataContract]
    public class RichPresenceParty
    {
        [JsonPropertyName("id")]
        [DataMember(Name = "id", Order = 1)]
        public string Id { get; set; }

        [JsonPropertyName("size")]
        [DataMember(Name = "size", Order = 2)]
        public int[] Size { get; set; }
    }
}