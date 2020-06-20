namespace Miki.Discord.Common.Events
{
    using System.Runtime.Serialization;

    [DataContract]
    public class DiscordMessageDeleteArgs
    {
        [DataMember(Name = "id")]
        public ulong MessageId { get; set; }

        [DataMember(Name = "channel_id")]
        public ulong ChannelId { get; set; }
    }
}