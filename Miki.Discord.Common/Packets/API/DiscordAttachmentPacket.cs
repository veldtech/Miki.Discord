using Newtonsoft.Json;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Text;

namespace Miki.Discord.Common.Packets
{
    [ProtoContract]
    public class DiscordAttachmentPacket
    {
        [JsonProperty("id")]
        [ProtoMember(1)]
        public ulong Id { get; set; }

        [JsonProperty("filename")]
        [ProtoMember(2)]
        public string FileName { get; set; }

        [JsonProperty("size")]
        [ProtoMember(3)]
        public int Size { get; set; }

        [JsonProperty("url")]
        [ProtoMember(4)]
        public string Url { get; set; }

        [JsonProperty("proxy_url")]
        [ProtoMember(5)]
        public string ProxyUrl { get; set; }

        [JsonProperty("height")]
        [ProtoMember(6)]
        public int? Height { get; set; }

        [JsonProperty("width")]
        [ProtoMember(7)]
        public int? Width { get; set; }

    }
}
