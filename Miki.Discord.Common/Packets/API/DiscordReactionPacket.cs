using System.Runtime.Serialization;

namespace Miki.Discord.Common
{
    [DataContract]
    public class DiscordReactionPacket
    {
        [DataMember(Name = "count")]
        public int Count;

        [DataMember(Name = "me")]
        public bool Me;

        [DataMember(Name = "emoji")]
        public DiscordEmoji Emoji;
    }
}