using System.Runtime.Serialization;

namespace Miki.Discord.Common.Packets.Events
{
    [DataContract]
    public class GuildEmojisUpdateEventArgs
	{
		[DataMember(Name ="guild_id")]
		public ulong GuildId;

		[DataMember(Name ="emojis")]
		public DiscordEmoji[] Emojis;
	}
}