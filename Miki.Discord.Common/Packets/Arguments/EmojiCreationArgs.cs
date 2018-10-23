using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Miki.Discord.Rest
{
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
