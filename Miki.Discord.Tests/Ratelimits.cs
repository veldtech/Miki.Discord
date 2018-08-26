using Miki.Cache;
using Miki.Cache.InMemory;
using Miki.Discord.Rest;
using Miki.Serialization.Protobuf;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Miki.Discord.Tests
{
    public class Ratelimits
    {
		private ICachePool _pool;

		public Ratelimits()
		{
			_pool = new InMemoryCachePool(
				new ProtobufSerializer()
			);
		}

		[Fact]
		public void IsRatelimited()
		{
			var p = _pool.GetAsync().Result;

			var rateLimit = new Ratelimit();

			rateLimit.Remaining = 5;
			rateLimit.Reset = (DateTimeOffset.Now + TimeSpan.FromSeconds(1)).ToUnixTimeSeconds();

			Assert.False(Ratelimit.IsRatelimited(rateLimit));

			rateLimit.Remaining = 0;

			Assert.True(Ratelimit.IsRatelimited(rateLimit));

			rateLimit.Global = 0;
			rateLimit.Remaining = 3;

			Assert.True(Ratelimit.IsRatelimited(rateLimit));

			rateLimit.Global = null;
			rateLimit.Remaining = 0;
			rateLimit.Reset = 0;

			Assert.False(Ratelimit.IsRatelimited(rateLimit));
		}
    }
}
