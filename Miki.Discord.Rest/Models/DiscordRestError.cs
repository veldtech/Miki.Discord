using Newtonsoft.Json;

namespace Miki.Discord.Rest
{
	public class DiscordRestError
	{
		[JsonProperty("code")]
		public int Code { get; set; }

		[JsonProperty("message")]
		public string Message { get; set; }

		public override string ToString()
		{
			return $"{Code}: {Message}\n";
		}
	}
}
