
using System.Runtime.Serialization;

namespace Miki.Discord.Common.Packets.Events
{
	[DataContract]
	public class MessageBulkDeleteEventArgs
	{
		[DataMember(Name = "guild_id")]
		public ulong guildId;

		[DataMember(Name = "channel_id")]
		public ulong channelId;

		[DataMember(Name = "ids")]
		public ulong[] messagesDeleted;
	}
}