using Miki.Cache;
using Miki.Net.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miki.Discord.Rest.Http
{
    public class DiscordRateLimiter : IRateLimiter
    {
        ICacheClient _cache;

        private string GetCacheKey(string route, string id)
            => $"discord:ratelimit:{route}:{id}";

        public DiscordRateLimiter(ICacheClient cache)
        {
            _cache = cache;
        }

        public async Task<bool> CanStartRequestAsync(RequestMethod method, string requestUri)
        {
            string key = GetCacheKey(requestUri.Split('/')[0], requestUri.Split('/')[1]);

            Ratelimit rateLimit = await _cache.GetAsync<Ratelimit>(key);
            if (rateLimit == null)
            {
                return true;
            }

            bool result = !rateLimit.IsRatelimited();
            rateLimit.Remaining--;

            await _cache.UpsertAsync(key, rateLimit);

            return result;
        }

        public async Task OnRequestSuccessAsync(HttpResponse response)
        {
            Uri requestUri = response.HttpResponseMessage.RequestMessage.RequestUri;
            string key = GetCacheKey(requestUri.AbsolutePath.Split('/')[2], requestUri.AbsolutePath.Split('/')[3]);

            if (response.HttpResponseMessage.Headers.Contains("X-RateLimit-Limit"))
            {
                var ratelimit = new Ratelimit();
                ratelimit.Remaining = int.Parse(response.HttpResponseMessage.Headers.GetValues("X-RateLimit-Remaining").ToList().FirstOrDefault());
                ratelimit.Limit = int.Parse(response.HttpResponseMessage.Headers.GetValues("X-RateLimit-Limit").ToList().FirstOrDefault());
                ratelimit.Reset = long.Parse(response.HttpResponseMessage.Headers.GetValues("X-RateLimit-Reset").ToList().FirstOrDefault());
                if (response.HttpResponseMessage.Headers.Contains("X-RateLimit-Global"))
                {
                    ratelimit.Global = int.Parse(response.HttpResponseMessage.Headers.GetValues("X-RateLimit-Global").ToList().FirstOrDefault());
                }
                await _cache.UpsertAsync(key, ratelimit);
            }
        }
    }
}
