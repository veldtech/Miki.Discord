
using System.Runtime.Serialization;

namespace Miki.Discord.Rest.Arguments
{
    using System.Text.Json.Serialization;

    [DataContract]
    public class ChannelBulkDeleteArgs
    {
        [JsonPropertyName("messages")]
        [DataMember(Name = "messages")]
        public ulong[] Messages { get; set; }

        public ChannelBulkDeleteArgs(ulong[] messages)
        {
            Messages = messages;
        }
    }
}