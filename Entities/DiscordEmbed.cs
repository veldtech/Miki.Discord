using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Miki.Discord.Rest.Entities
{
    public class DiscordEmbed
    {
		[JsonProperty("title")]
		public string Title { get; set; }

		[JsonProperty("description")]
		public string Description { get; set; }

		[JsonProperty("color")]
		public int Color;

		[JsonProperty("fields")]
		public List<EmbedField> Fields { get; set; }

		[JsonProperty("author")]
		public EmbedAuthor Author;

		[JsonProperty("footer")]
		public EmbedFooter Footer;

		[JsonProperty("thumbnail")]
		public EmbedImage Thumbnail;

		[JsonProperty("image")]
		public EmbedImage Image;
	}

	public class EmbedAuthor
	{
		public string Name { get; set; }
		public string IconUrl { get; set; }
		public string Url { get; set; }
	}

	public class EmbedFooter
	{
		[JsonProperty("icon_url")]
		public string IconUrl { get; set; }

		[JsonProperty("text")]
		public string Text { get; set; }
	}

	public class EmbedImage
	{
		[JsonProperty("url")]
		public string Url { get; set; }
	}

	public class EmbedField
	{
		[JsonProperty("name")]
		public string Title { get; set; }

		[JsonProperty("value")]
		public string Content { get; set; }

		[JsonProperty("inline")]
		public bool Inline { get; set; } = false;
	}
}
