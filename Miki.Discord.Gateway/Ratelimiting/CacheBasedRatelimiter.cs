using Miki.Cache;
using System;
using System.Threading.Tasks;

namespace Miki.Discord.Gateway.Ratelimiting
{
    public class CacheBasedRatelimiter : IGatewayRatelimiter
    {
        private const string CacheKey = "miki:gateway:identify:ratelimit";

        private readonly ICacheClient cache;

        public CacheBasedRatelimiter(ICacheClient cache)
        {
            this.cache = cache;
        }

        public async Task<bool> CanIdentifyAsync()
        {
            var ticks = await cache.GetAsync<long>(CacheKey);
            if(ticks == 0)
            {
                await cache.UpsertAsync(CacheKey, DateTime.UtcNow.AddSeconds(5.1).Ticks);
                return true;
            }

            if(DateTime.UtcNow.Ticks > ticks)
            {
                await cache.UpsertAsync(CacheKey, DateTime.UtcNow.AddSeconds(5.1).Ticks);
                return true;
            }
            return false;
        }
    }
}
