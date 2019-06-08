using Miki.Cache;
using Miki.Discord.Common;

namespace Miki.Discord
{
	public class DiscordClientConfigurations
	{
		public IApiClient ApiClient;
		public IGateway Gateway;
        public IExtendedCacheClient CacheClient;
    }
}