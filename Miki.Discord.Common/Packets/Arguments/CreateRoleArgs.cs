using Newtonsoft.Json;

namespace Miki.Discord.Common
{
	public class CreateRoleArgs
	{
		[JsonProperty("name")]
		public string Name;

		[JsonProperty("permissions")]
		public GuildPermission? Permissions;

		[JsonProperty("color")]
		public int? Color;

		[JsonProperty("hoist")]
		public bool? Hoisted;

		[JsonProperty("mentionable")]
		public bool? Mentionable;
	}
}