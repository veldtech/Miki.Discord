
using System.Runtime.Serialization;

namespace Miki.Discord.Rest.Arguments
{
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