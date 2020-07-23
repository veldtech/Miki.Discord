using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Miki.Discord.Rest.Arguments
{
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