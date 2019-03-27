using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Miki.Discord.Common.Packets
{
    [ProtoContract]
    public class DiscordAttachmentPacket
    {
        [DataMember(Name ="id")]
        [ProtoMember(1)]
        public ulong Id { get; set; }

        [DataMember(Name ="filename")]
        [ProtoMember(2)]
        public string FileName { get; set; }

        [DataMember(Name ="size")]
        [ProtoMember(3)]
        public int Size { get; set; }

        [DataMember(Name ="url")]
        [ProtoMember(4)]
        public string Url { get; set; }

        [DataMember(Name ="proxy_url")]
        [ProtoMember(5)]
        public string ProxyUrl { get; set; }

        [DataMember(Name ="height")]
        [ProtoMember(6)]
        public int? Height { get; set; }

        [DataMember(Name ="width")]
        [ProtoMember(7)]
        public int? Width { get; set; }

    }
}
