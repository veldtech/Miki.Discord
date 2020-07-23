using Miki.Cache;
using Miki.Net.Http;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Miki.Discord.Rest.Http
{
    public class DiscordRateLimiter : IRateLimiter
    {
        private readonly ICacheClient cache;

        private const string LimitHeader = "X-RateLimit-Limit";
        private const string RemainingHeader = "X-RateLimit-Remaining";
        private const string ResetHeader = "X-RateLimit-Reset";
        private const string GlobalHeader = "X-RateLimit-Global";

        private string GetCacheKey(string route, string id)
            => $"discord:ratelimit:{route}:{id}";

        public DiscordRateLimiter(ICacheClient cache)
        {
            this.cache = cache;
        }

        public async Task<bool> CanStartRequestAsync(RequestMethod method, string requestUri)
        {
            string key = GetCacheKey(requestUri.Split('/')[0], requestUri.Split('/')[1]);

            Ratelimit rateLimit = await cache.GetAsync<Ratelimit>(key);
            rateLimit.Remaining--;

            await cache.UpsertAsync(key, rateLimit);

            return !rateLimit.IsRatelimited();
        }

        public async Task OnRequestSuccessAsync(HttpResponse response)
        {
            var httpMessage = response.HttpResponseMessage;

            Uri requestUri = httpMessage.RequestMessage.RequestUri;
            string[] paths = requestUri.AbsolutePath.Split('/');
            string key = GetCacheKey(paths[2], paths[3]);

            if(httpMessage.Headers.Contains(LimitHeader))
            {
                var ratelimit = new Ratelimit();
                if(httpMessage.Headers.TryGetValues(RemainingHeader, out var values))
                {
                    ratelimit.Remaining = int.Parse(values.FirstOrDefault());
                }

                if(httpMessage.Headers.TryGetValues(LimitHeader, out var limitValues))
                {
                    ratelimit.Limit = int.Parse(limitValues.FirstOrDefault());
                }

                if(httpMessage.Headers.TryGetValues(ResetHeader, out var resetValues))
                {
                    ratelimit.Reset = int.Parse(resetValues.FirstOrDefault());
                }

                if(httpMessage.Headers.TryGetValues(GlobalHeader, out var globalValues))
                {
                    ratelimit.Global = int.Parse(globalValues.FirstOrDefault());
                }

                await cache.UpsertAsync(key, ratelimit);
            }
        }
    }
}
