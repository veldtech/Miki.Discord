using System.Runtime.Serialization;

namespace Miki.Discord.Common
{
	[DataContract]
	public class EditMessageArgs
	{
		public EditMessageArgs(string content = null, DiscordEmbed embed = null)
		{
			Content = content;
			Embed = embed;
		}

		[DataMember(Name = "content")]
		public string Content;

		[DataMember(Name = "embed")]
		public DiscordEmbed Embed;
	}

	[DataContract]
	public class MessageArgs : EditMessageArgs
	{
		public MessageArgs(string content = null, DiscordEmbed embed = null, bool tts = false)
			: base(content, embed)
		{
			TextToSpeech = tts;
		}

		[DataMember(Name = "tts")]
		public bool TextToSpeech;
	}
}