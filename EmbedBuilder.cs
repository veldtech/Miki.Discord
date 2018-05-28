using Miki.Discord.Rest.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Miki.Discord.Rest
{
    public class EmbedBuilder
    {
		DiscordEmbed embed = new DiscordEmbed();

		public EmbedBuilder AddField(string title, string content, bool isInline = false)
		{
			if (embed.Fields == null)
			{
				embed.Fields = new List<EmbedField>();
			}

			embed.Fields.Add(new EmbedField()
			{
				Title = title,
				Content = content,
				Inline = isInline
			});

			return this;
		}

		public EmbedBuilder AddInlineField(string title, string content)
		{
			return AddField(title, content, true);
		}

		public EmbedBuilder SetColor(int r, int g, int b)
		{
			embed.Color = Color.ToHex(r, g, b);
			return this;
		}
		public EmbedBuilder SetColor(Color color)
		{
			embed.Color = color?.ToHex() ?? 0;
			return this;
		}

		public EmbedBuilder SetDescription(string description)
		{
			embed.Description = description;
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

		public static EmbedBuilder CreateSimple(string title, string description, Color color = null)
		{
			return new EmbedBuilder()
				.SetTitle(title)
				.SetDescription(description)
				.SetColor(color);
		}

		internal DiscordEmbed ToEmbed()
		{
			return embed;
		}
	}
}
