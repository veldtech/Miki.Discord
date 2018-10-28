using Newtonsoft.Json;

namespace Miki.Discord.Common.Events
{
	public class MessageDeleteArgs
	{
		[JsonProperty("id")]
		public ulong MessageId { get; set; }

		[JsonProperty("channel_id")]
		public ulong ChannelId { get; set; }
	}
}