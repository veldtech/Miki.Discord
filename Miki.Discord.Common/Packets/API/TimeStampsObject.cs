using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Miki.Discord.Common
{
    [DataContract]
    public class TimeStampsObject
    {
        [JsonPropertyName("start")]
        [DataMember(Name = "start", Order = 1)]
        public long Start { get; set; }

        [JsonPropertyName("end")]
        [DataMember(Name = "end", Order = 2)]
        public long End { get; set; }
    }
}