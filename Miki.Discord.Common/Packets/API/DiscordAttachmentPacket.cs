
namespace Miki.Discord.Common
{
    using System.Runtime.Serialization;
    using System.Text.Json.Serialization;

    [DataContract]
    public class DiscordAttachmentPacket
    {
        [JsonPropertyName("id")]
        [DataMember(Name = "id", Order = 1)]
        public ulong Id { get; set; }

        [JsonPropertyName("filename")]
        [DataMember(Name = "filename", Order = 2)]
        public string FileName { get; set; }

        [JsonPropertyName("size")]
        [DataMember(Name = "size", Order = 3)]
        public int Size { get; set; }

        [JsonPropertyName("url")]
        [DataMember(Name = "url", Order = 4)]
        public string Url { get; set; }

        [JsonPropertyName("proxy_url")]
        [DataMember(Name = "proxy_url", Order = 5)]
        public string ProxyUrl { get; set; }

        [JsonPropertyName("height")]
        [DataMember(Name = "height", Order = 6)]
        public int? Height { get; set; }

        [JsonPropertyName("width")]
        [DataMember(Name = "width", Order = 7)]
        public int? Width { get; set; }

    }
}
