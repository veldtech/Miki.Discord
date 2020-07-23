using System.IO;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Miki.Discord.Rest;

namespace Miki.Discord.Common
{
    /// <summary>
    /// Data structure to create an emoji.
    /// </summary>
    [DataContract]
    public class EmojiCreationArgs : EmojiModifyArgs
    {
        [JsonPropertyName("image")]
        [DataMember(Name = "image")]
        public Stream Image { get; set; }

        public EmojiCreationArgs(string name, Stream image, params ulong[] roles)
            : base(name, roles)
        {
            Image = image;
        }
    }
}