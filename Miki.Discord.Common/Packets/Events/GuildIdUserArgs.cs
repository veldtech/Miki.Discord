using System.Runtime.Serialization;

namespace Miki.Discord.Common.Packets
{
    [DataContract]
    public class GuildIdUserArgs
	{
		[DataMember(Name ="user")]
		public DiscordUserPacket User;

		[DataMember(Name ="guild_id")]
		public ulong GuildId;
	}
}