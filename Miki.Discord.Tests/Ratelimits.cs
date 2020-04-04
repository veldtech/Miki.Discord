namespace Miki.Discord.Tests
{
    using Miki.Discord.Rest;
    using System;
    using Xunit;

    public class Ratelimits
    {
        [Fact]
        public void IsRatelimited()
        {
            var rateLimit = new Ratelimit
            {
                Remaining = 5,
                Reset = (DateTimeOffset.Now + TimeSpan.FromSeconds(1)).ToUnixTimeSeconds()
            };

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
