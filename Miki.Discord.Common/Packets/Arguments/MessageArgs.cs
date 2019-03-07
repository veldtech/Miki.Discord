namespace Miki.Discord.Common
{
	public class MessageArgs : EditMessageArgs
	{
		public bool tts = false;
	}

	public class EditMessageArgs
	{
		public string content;
		public DiscordEmbed embed;
	}
}