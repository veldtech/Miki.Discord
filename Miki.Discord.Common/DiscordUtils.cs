using System;

namespace Miki.Discord.Common
{
	public static class DiscordUtils
	{
		public const long DiscordEpoch = 1420070400000;

		public static DateTime GetCreationTime(this ISnowflake snowflake)
		{
			return new DateTime(2015, 1, 1, 0, 0, 0) + TimeSpan.FromMilliseconds((long)(snowflake.Id >> 22));
		}
	}
}