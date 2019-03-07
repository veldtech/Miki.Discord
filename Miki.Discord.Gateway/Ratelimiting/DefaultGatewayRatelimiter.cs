using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Miki.Discord.Gateway.Ratelimiting
{
    public class DefaultGatewayRatelimiter : IGatewayRatelimiter
    {
        private DateTime lastIdentifyAccepted = DateTime.MinValue;

        public Task<bool> CanIdentifyAsync()
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
