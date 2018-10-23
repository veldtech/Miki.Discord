using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Miki.Discord.Rest
{
    public class EmojiModifyArgs
    {
		[JsonProperty("name")]
		public string Name { get; private set; }

		[JsonProperty("roles")]
		public ulong[] Roles { get; private set; }

		public EmojiModifyArgs(string name, params ulong[] roles)
		{
			Name = name;
			Roles = roles;
		}
	}
}
