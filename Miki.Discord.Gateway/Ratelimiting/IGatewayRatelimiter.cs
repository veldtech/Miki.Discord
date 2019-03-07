using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Miki.Discord.Gateway.Ratelimiting
{
    public interface IGatewayRatelimiter
    {
        Task<bool> CanIdentifyAsync();
    }
}
