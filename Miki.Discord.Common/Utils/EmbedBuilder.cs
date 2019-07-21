using Miki.Discord.Common;
using Miki.Discord.Rest;
using System.Collections.Generic;

namespace Miki.Discord
{
	public class EmbedBuilder
	{
		private readonly DiscordEmbed embed = new DiscordEmbed();

        public EmbedAuthor Author
		{
			get => embed.Author;
			set => embed.Author = value;
		}

		public Color Color
		{
			get => new Color(embed.Color ?? 0U);
			set => embed.Color = value.Value;
		}

        public string Description
		{
			get => embed.Description;
			set => embed.Description = value;
		}

		public EmbedFooter Footer
		{
			get => embed.Footer;
			set => embed.Footer = value;
		}

        public string ImageUrl
		{
			get => embed.Image?.Url ?? null;
			set => embed.Image = new EmbedImage() { Url = value };
		}

        public string ThumbnailUrl
        {
            get => embed.Thumbnail?.Url ?? null;
            set => embed.Thumbnail = new EmbedImage() { Url = value };
        }

        public string Title
		{
			get => embed.Title;
			set => embed.Title = value;
		}

		public EmbedBuilder AddField(string title, object content, bool isInline = false)
		{
			if (embed.Fields == null)
			{
				embed.Fields = new List<EmbedField>();
			}

			embed.Fields.Add(new EmbedField()
			{
				Title = title,
				Content = content.ToString(),
				Inline = isInline
			});

			return this;
		}

		public EmbedBuilder AddInlineField(string title, string content)
            => AddField(title, content, true);

		public EmbedBuilder SetAuthor(string name, string iconUrl = null, string url = null)
		{
			embed.Author = new EmbedAuthor()
			{
				Name = name,
				IconUrl = iconUrl,
				Url = url
			};
			return this;
		}

		public EmbedBuilder SetColor(int r, int g, int b)
		{
			embed.Color = new Color(r, g, b).Value;
			return this;
		}

		public EmbedBuilder SetColor(float r, float g, float b)
		{
			return SetColor(new Color(r, g, b));
		}

		public EmbedBuilder SetColor(Color color)
		{
			embed.Color = color.Value;
			return this;
		}

		public EmbedBuilder SetDescription(string description)
		{
			if (!string.IsNullOrWhiteSpace(description))
			{
				embed.Description = description;
			}
			return this;
		}

		public EmbedBuilder SetFooter(string text, string url = "")
		{
			embed.Footer = new EmbedFooter()
			{
				Text = text,
				IconUrl = url
			};

			return this;
		}

		public EmbedBuilder SetImage(string url)
		{
			embed.Image = new EmbedImage()
			{
				Url = url
			};

			return this;
		}

		public EmbedBuilder SetTitle(string title)
		{
			embed.Title = title;
			return this;
		}

		public EmbedBuilder SetThumbnail(string url)
		{
			embed.Thumbnail = new EmbedImage()
			{
				Url = url
			};

			return this;
		}

		public DiscordEmbed ToEmbed()
		{
			return embed;
		}
	}
}