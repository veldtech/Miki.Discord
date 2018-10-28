using Newtonsoft.Json;

namespace Miki.Discord.Common.Events
{
	public class ModifyGuildMemberArgs
	{
		[JsonProperty("nick")]
		public string Nickname;

		[JsonProperty("roles")]
		public ulong[] RoleIds;

		[JsonProperty("mute")]
		public bool? Muted;

		[JsonProperty("deaf")]
		public bool? Deafened;

		[JsonProperty("channel_id")]
		public ulong? MoveToChannelId;
	}
}