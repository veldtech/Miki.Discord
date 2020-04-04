namespace Miki.Discord.Common
{
    using System.IO;
    using System.Runtime.Serialization;
    using Miki.Discord.Rest;

    /// <summary>
    /// Data structure to create an emoji.
    /// </summary>
    [DataContract]
    public class EmojiCreationArgs : EmojiModifyArgs
    {
        [DataMember(Name = "image")]
        public Stream Image { get; private set; }

        public EmojiCreationArgs(string name, Stream image, params ulong[] roles)
            : base(name, roles)
        {
            Image = image;
        }
    }
}