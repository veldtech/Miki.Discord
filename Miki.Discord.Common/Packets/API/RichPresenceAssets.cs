using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Miki.Discord.Common
{
    [DataContract]
    public class RichPresenceAssets
    {
        [JsonPropertyName("large_image")]
        [DataMember(Name = "large_image", Order = 1)]
        public string LargeImage { get; set; }

        [JsonPropertyName("large_text")]
        [DataMember(Name = "large_text", Order = 2)]
        public string LargeText { get; set; }

        [JsonPropertyName("small_image")]
        [DataMember(Name = "small_image", Order = 3)]
        public string SmallImage { get; set; }

        [JsonPropertyName("small_text")]
        [DataMember(Name = "small_text", Order = 4)]
        public string SmallText { get; set; }
    }
}