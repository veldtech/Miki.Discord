using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Miki.Discord.Common
{
    [DataContract]
    public class DiscordEmbed
	{
		[DataMember(Name ="title")]
		public string Title { get; set; }

        [DataMember(Name = "description")]
		public string Description { get; set; }

		[DataMember(Name ="color")]
		public uint? Color { get; set; } = null;

		[DataMember(Name ="fields")]
		public List<EmbedField> Fields { get; set; }

		[DataMember(Name ="author")]
		public EmbedAuthor Author;

		[DataMember(Name ="footer")]
		public EmbedFooter Footer;

		[DataMember(Name ="thumbnail")]
		public EmbedImage Thumbnail;

		[DataMember(Name ="image")]
		public EmbedImage Image;
	}

    [DataContract]
    public class EmbedAuthor
	{
		[DataMember(Name ="name")]
		public string Name { get; set; }

		[DataMember(Name ="icon_url")]
		public string IconUrl { get; set; }

		[DataMember(Name ="url")]
		public string Url { get; set; }
	}

    [DataContract]
    public class EmbedFooter
	{
		[DataMember(Name ="icon_url")]
		public string IconUrl { get; set; }

		[DataMember(Name ="text")]
		public string Text { get; set; }
	}

    [DataContract]
    public class EmbedImage
	{
		[DataMember(Name ="url")]
		public string Url { get; set; }
	}

    [DataContract]
    public class EmbedField
	{
		[DataMember(Name ="name")]
		public string Title { get; set; }

		[DataMember(Name ="value")]
		public string Content { get; set; }

		[DataMember(Name ="inline")]
		public bool Inline { get; set; } = false;
	}
}