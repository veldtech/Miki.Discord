using System;
using System.Collections.Generic;
using System.Text;

namespace Miki.Discord.Common
{
    public static class DiscordHelper
    {
		public const string DiscordUrl = "https://discordapp.com";
		public const string BaseUrl = "/api/v6";
		public const string CdnUrl = "https://cdn.discordapp.com";

		public static string GetAvatarUrl(ulong id, string hash)
			=> $"{CdnUrl}/avatars/{id}/{hash}.png";

		public static string GetAvatarUrl(ushort discriminator)
			=> $"{CdnUrl}/avatars/{discriminator}.png";
	}
}
