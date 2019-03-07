using Newtonsoft.Json;
using System.IO;

namespace Miki.Discord.Rest
{
    /// <summary>
    /// Data structure to create an emoji.
    /// </summary>
	public class EmojiCreationArgs : EmojiModifyArgs
	{
		[JsonProperty("image")]
		public Stream Image { get; private set; }

		public EmojiCreationArgs(string name, Stream image, params ulong[] roles)
			: base(name, roles)
		{
			Image = image;
		}
	}
}