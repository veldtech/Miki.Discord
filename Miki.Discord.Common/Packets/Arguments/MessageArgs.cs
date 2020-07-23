using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Miki.Discord.Common
{
    [DataContract]
    public class EditMessageArgs
    {
        public EditMessageArgs(string content = null, DiscordEmbed embed = null)
        {
            Content = content;
            Embed = embed;
        }

        [JsonPropertyName("channels")]
        [DataMember(Name = "content")]
        public string Content { get; set; }

        [JsonPropertyName("channels")]
        [DataMember(Name = "embed")]
        public DiscordEmbed Embed { get; set; }
    }

    [DataContract]
    public class MessageArgs : EditMessageArgs
    {
        public MessageArgs(string content = null, DiscordEmbed embed = null, bool tts = false)
            : base(content, embed)
        {
            TextToSpeech = tts;
        }

        [JsonPropertyName("tts")]
        [DataMember(Name = "tts")]
        public bool TextToSpeech { get; set; }
    }
}