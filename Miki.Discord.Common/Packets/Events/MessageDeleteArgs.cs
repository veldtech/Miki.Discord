using System.Runtime.Serialization;

namespace Miki.Discord.Common.Events
{
	public class MessageDeleteArgs
	{
		[DataMember(Name ="id")]
		public ulong MessageId { get; set; }

		[DataMember(Name ="channel_id")]
		public ulong ChannelId { get; set; }
	}
}