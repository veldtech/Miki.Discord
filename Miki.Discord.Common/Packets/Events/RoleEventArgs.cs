using Miki.Discord.Common.Packets;
using System.Runtime.Serialization;

namespace Miki.Discord.Common.Events
{
    [DataContract]
    public class RoleEventArgs
	{
		[DataMember(Name ="guild_id")]
		public ulong GuildId;

		[DataMember(Name ="role")]
		public DiscordRolePacket Role;
	}
}