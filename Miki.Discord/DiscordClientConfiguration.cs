#nullable enable

using Miki.Cache;
using Miki.Discord.Common;

namespace Miki.Discord
{
    /// <summary>
    /// Configuration properties for DiscordClient.
    /// </summary>
    public class DiscordClientConfiguration
    {
        /// <summary>
        /// Creates a new Configuration setup. 
        /// </summary>
        public DiscordClientConfiguration(
            IApiClient apiClient, IGateway gateway, IExtendedCacheClient cache)
        {
            ApiClient = apiClient;
            Gateway = gateway;
            CacheClient = cache;
        }
        
        public IApiClient ApiClient { get; }
        
        public IGateway Gateway { get; }

        public IExtendedCacheClient CacheClient { get; }
    }
}