using Newtonsoft.Json;

namespace Miki.Discord.Rest.Arguments
{
	public class ChannelBulkDeleteArgs
	{
		[JsonProperty("messages")]
		public ulong[] Messages { get; set; }

		public ChannelBulkDeleteArgs(ulong[] messages)
		{
			Messages = messages;
		}
	}
}