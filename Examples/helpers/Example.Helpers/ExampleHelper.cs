using Miki.Discord.Common;
using Miki.Logging;
using System;

namespace Example.Helpers
{
    public static class ExampleHelper
    {
        /// <summary>
        /// Enables library wide logging for all Miki libraries. Consider using this logging for
        /// debugging or general information.
        /// </summary>
        public static void InitLog(LogLevel level)
        {
            new LogBuilder().AddLogEvent(((message, thisLevel) =>
            {
                if (thisLevel >= level)
                {
                    Console.WriteLine(level + " | " + message);
                }
            })).Apply();
        }

        public static DiscordToken GetTokenFromEnv()
        {
            return Environment.GetEnvironmentVariable("TOKEN")
              ?? throw new InvalidOperationException("Token environment value was not passed.");
        }
    }
}
