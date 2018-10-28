using Miki.Cache;
using Miki.Rest;
using System;
using System.Linq;
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
				await cache.UpsertAsync(key, rateLimit);
			}

			if (!Ratelimit.IsRatelimited(rateLimit))
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
				await cache.UpsertAsync(key, rateLimit);
			}

			if (!Ratelimit.IsRatelimited(rateLimit))
			{
				var response = await t();
				await HandleRateLimit(cache, response, rateLimit, key);
				return response;
			}
			return default(RestResponse<T>);
		}

		private static async Task HandleRateLimit(ICacheClient cache, RestResponse rc, Ratelimit ratelimit, string key)
		{
			if (!Ratelimit.IsRatelimited(ratelimit))
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
					await cache.UpsertAsync(key, ratelimit);
				}
			}
		}
	}
}