namespace Miki.Discord.Gateway.Ratelimiting
{
    using System;
    using System.Threading.Tasks;
    using System.Threading;

    public class DefaultGatewayRatelimiter : IGatewayRatelimiter
    {
        private DateTime lastIdentifyAccepted = DateTime.MinValue;

        public Task<bool> CanIdentifyAsync(CancellationToken token)
        {
            if(lastIdentifyAccepted.AddSeconds(5) > DateTime.Now)
            {
                return Task.FromResult(false);
            }
            lastIdentifyAccepted = DateTime.Now;
            return Task.FromResult(true);
        }
    }
}
