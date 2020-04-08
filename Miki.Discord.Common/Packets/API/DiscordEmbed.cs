namespace Miki.Discord.Common
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using System.Text.Json.Serialization;

    [Serializable]
    [DataContract]
    public sealed class DiscordEmbed
    {
        [JsonPropertyName("title")]
        [DataMember(Name = "title")]
        public string Title { get; set; }

        [JsonPropertyName("description")]
        [DataMember(Name = "description")]
        public string Description { get; set; }

        [JsonPropertyName("color")]
        [DataMember(Name = "color")]
        public uint? Color { get; set; }

        [JsonPropertyName("fields")]
        [DataMember(Name = "fields")]
        public List<EmbedField> Fields { get; set; }

        [JsonPropertyName("author")]
        [DataMember(Name = "author")]
        public EmbedAuthor Author { get; set; }

        [JsonPropertyName("footer")]
        [DataMember(Name = "footer")]
        public EmbedFooter Footer { get; set; }

        [JsonPropertyName("thumbnail")]
        [DataMember(Name = "thumbnail")]
        public EmbedImage Thumbnail { get; set; }

        [JsonPropertyName("image")]
        [DataMember(Name = "image")]
        public EmbedImage Image { get; set; }
    }

    [DataContract]
    public class EmbedAuthor
    {
        [JsonPropertyName("name")]
        [DataMember(Name = "name")]
        public string Name { get; set; }

        [JsonPropertyName("icon_url")]
        [DataMember(Name = "icon_url")]
        public string IconUrl { get; set; }

        [JsonPropertyName("url")]
        [DataMember(Name = "url")]
        public string Url { get; set; }
    }

    [DataContract]
    public class EmbedFooter
    {
        [JsonPropertyName("icon_url")]
        [DataMember(Name = "icon_url")]
        public string IconUrl { get; set; }

        [JsonPropertyName("text")]
        [DataMember(Name = "text")]
        public string Text { get; set; }
    }

    [DataContract]
    public class EmbedImage
    {
        [JsonPropertyName("url")]
        [DataMember(Name = "url")]
        public string Url { get; set; }
    }

    [DataContract]
    public class EmbedField
    {
        [JsonPropertyName("name")]
        [DataMember(Name = "name")]
        public string Title { get; set; }

        [JsonPropertyName("value")]
        [DataMember(Name = "value")]
        public string Content { get; set; }

        [JsonPropertyName("inline")]
        [DataMember(Name = "inline")]
        public bool Inline { get; set; } = false;
    }
}