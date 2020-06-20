namespace Miki.Discord
{
    using Miki.Cache;
    using Miki.Discord.Cache;
    using Miki.Discord.Common;
    using Miki.Functional;

    public class DiscordClientConfigurations
    {
        public IApiClient ApiClient { get; set; }
        
        public IGateway Gateway { get; set; }

        public IExtendedCacheClient CacheClient { get; set; }

        /// <summary>
        /// Optional cache handler implementation. If none provided, it will initialize with
        /// <see cref="DefaultCacheHandler"/> class.
        /// </summary>
        public Optional<ICacheHandler> CacheHandler { get; set; }
    }
}