
using System.Runtime.Serialization;

namespace Miki.Discord.Rest.Arguments
{
    [DataContract]
    public class ChannelBulkDeleteArgs
	{
		[DataMember(Name ="messages")]
		public ulong[] Messages { get; set; }

		public ChannelBulkDeleteArgs(ulong[] messages)
		{
			Messages = messages;
		}
	}
}