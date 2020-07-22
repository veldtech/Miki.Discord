using Miki.Cache;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Miki.Discord.Gateway.Ratelimiting
{
    public class CacheBasedRatelimiter : IGatewayRatelimiter
    {
        private const string CacheKey = "miki:gateway:identify:ratelimit";

        private readonly IDistributedLockProvider cache;
        private readonly bool largeBot;

        public CacheBasedRatelimiter(IDistributedLockProvider cache, bool largeBot = false)
        {
            this.cache = cache;
            this.largeBot = largeBot;
        }

        /// <inheritdoc/>
        public async Task<bool> CanIdentifyAsync(int shardId, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            try
            {
                await cache.AcquireLockAsync(GetCacheKey(shardId), token);
            }
            catch(OperationCanceledException)
            {
                return false;
            }

            await cache.ExpiresAsync(GetCacheKey(shardId), TimeSpan.FromSeconds(5));
            return true;
        }

        private string GetCacheKey(int shardId)
        {
            if(largeBot)
            {
                return $"{CacheKey}:{shardId % 16}";
            }
            return CacheKey;
        }
    }
}
