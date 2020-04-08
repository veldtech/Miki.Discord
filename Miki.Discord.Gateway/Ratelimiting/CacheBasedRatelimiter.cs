namespace Miki.Discord.Gateway.Ratelimiting
{
    using Miki.Cache;
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    public class CacheBasedRatelimiter : IGatewayRatelimiter
    {
        private const string CacheKey = "miki:gateway:identify:ratelimit";

        private readonly IDistributedLockProvider cache;

        public CacheBasedRatelimiter(IDistributedLockProvider cache)
        {
            this.cache = cache;
        }

        public async Task<bool> CanIdentifyAsync(CancellationToken token)
        {
            try
            {
                await cache.AcquireLockAsync(CacheKey, token);
            }
            catch(OperationCanceledException)
            {
                return false;
            }

            await cache.ExpiresAsync(CacheKey, TimeSpan.FromSeconds(5));
            return true;
        }
    }
}
