using Miki.Rest;
using StackExchange.Redis.Extensions.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miki.Discord.Rest
{
    public static class RatelimitHelper
    {
		public static async Task ProcessRateLimitedAsync(string redisId, ICacheClient cache, Func<Task<RestResponse>> t)
		{
			string key = $"discord:ratelimit:" + redisId;
			Ratelimit rateLimit = await cache.GetAsync<Ratelimit>(key);
			if (rateLimit != null)
			{
				rateLimit.Remaining--;
				await cache.AddAsync(key, rateLimit);
			}

			if (!IsRatelimited(rateLimit))
			{
				var response = await t();
				await HandleRateLimit(cache, response, rateLimit, key);
			}
		}
		public static async Task<RestResponse<T>> ProcessRateLimitedAsync<T>(string redisId, ICacheClient cache, Func<Task<RestResponse<T>>> t)
		{
			string key = $"discord:ratelimit:" + redisId;
			Ratelimit rateLimit = await cache.GetAsync<Ratelimit>(key);
			if (rateLimit != null)
			{
				rateLimit.Remaining--;
				await cache.AddAsync(key, rateLimit);
			}

			if(!IsRatelimited(rateLimit))
			{
				var response = await t();
				await HandleRateLimit(cache, response, rateLimit, key);
				return response;
			}
			return default(RestResponse<T>);
		}

		private static async Task HandleRateLimit(ICacheClient cache, RestResponse rc, Ratelimit ratelimit, string key)
		{
			if (!IsRatelimited(ratelimit))
			{
				if (rc.HttpResponseMessage.Headers.Contains("X-RateLimit-Limit"))
				{
					ratelimit = new Ratelimit();
					ratelimit.Remaining = int.Parse(rc.HttpResponseMessage.Headers.GetValues("X-RateLimit-Remaining").ToList().FirstOrDefault());
					ratelimit.Limit = int.Parse(rc.HttpResponseMessage.Headers.GetValues("X-RateLimit-Limit").ToList().FirstOrDefault());
					ratelimit.Reset = long.Parse(rc.HttpResponseMessage.Headers.GetValues("X-RateLimit-Reset").ToList().FirstOrDefault());
					if (rc.HttpResponseMessage.Headers.Contains("X-RateLimit-Global"))
					{
						ratelimit.Global = int.Parse(rc.HttpResponseMessage.Headers.GetValues("X-RateLimit-Global").ToList().FirstOrDefault());
					}
					await cache.AddAsync(key, ratelimit);
				}
			}
		}

		private static bool IsRatelimited(Ratelimit rl)
		{
			return (rl?.Remaining ?? 1) <= 0 && DateTime.UtcNow <= DateTimeOffset.FromUnixTimeSeconds(rl?.Reset ?? 0);
		}
	}
}
