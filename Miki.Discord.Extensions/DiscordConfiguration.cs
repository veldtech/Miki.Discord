using Miki.Discord.Common;
using Miki.Discord.Gateway;

namespace Miki.Discord.Extensions
{
    public class DiscordConfiguration
    {
        public DiscordToken Token { get; set; }

        public GatewayProperties GatewayProperties { get; set; } = new GatewayProperties();
    }
}