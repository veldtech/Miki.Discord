using Newtonsoft.Json;

namespace Miki.Discord.Common.Events
{
	public class RoleDeleteEventArgs
	{
		[JsonProperty("guild_id")]
		public ulong GuildId;

		[JsonProperty("role_id")]
		public ulong RoleId;
	}
}