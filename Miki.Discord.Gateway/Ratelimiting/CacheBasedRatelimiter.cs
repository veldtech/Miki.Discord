using Miki.Cache;
using System;
using System.Threading.Tasks;

namespace Miki.Discord.Gateway.Ratelimiting
{
    public class CacheBasedRatelimiter : IGatewayRatelimiter
    {
        const string CacheKey = "miki:gateway:identify:ratelimit";

        ICacheClient _cache;

        public CacheBasedRatelimiter(ICacheClient cache)
        {
            _cache = cache;
        }

        public async Task<bool> CanIdentifyAsync()
        {
            var ticks = await _cache.GetAsync<long>(CacheKey);
            if(ticks == 0)
            {
                await _cache.UpsertAsync(CacheKey, DateTime.UtcNow.AddSeconds(5.1).Ticks);
                return true;
            }

            if(DateTime.UtcNow.Ticks > ticks)
            {
                await _cache.UpsertAsync(CacheKey, DateTime.UtcNow.AddSeconds(5.1).Ticks);
                return true;
            }
            return false;
        }
    }
}
