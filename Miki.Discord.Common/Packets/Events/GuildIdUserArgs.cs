using System.Runtime.Serialization;

namespace Miki.Discord.Common.Packets
{
    [DataContract]
    public class GuildIdUserArgs
	{
		[DataMember(Name ="user")]
		public DiscordUserPacket user;

		[DataMember(Name ="guild_id")]
		public ulong guildId;
	}
}