using System;
using System.Threading.Tasks;
using System.Threading;
using System.Linq;

namespace Miki.Discord.Gateway.Ratelimiting
{
    public class DefaultGatewayRatelimiter : IGatewayRatelimiter
    {
        private readonly DateTime[] lastIdentifyAccepted;
        private readonly bool largeBot;

        public DefaultGatewayRatelimiter(bool largeBot = false)
        {
            lastIdentifyAccepted = new DateTime[largeBot ? 16 : 1];
            this.largeBot = largeBot;
        }

        /// <inheritdoc/>
        public Task<bool> CanIdentifyAsync(int shardId, CancellationToken token)
        {
            if(GetLastIdentify(shardId).AddSeconds(5) > DateTime.UtcNow)
            {
                return Task.FromResult(false);
            }
            UpdateLastIdentify(shardId);
            return Task.FromResult(true);
        }

        private DateTime GetLastIdentify(int shardId)
        {
            if(largeBot)
            {
                return lastIdentifyAccepted[shardId % 16];
            }
            return lastIdentifyAccepted.First();
        }

        private void UpdateLastIdentify(int shardId)
        {
            if (largeBot)
            {
                lastIdentifyAccepted[shardId % 16] = DateTime.UtcNow;
                return;
            }
            lastIdentifyAccepted[0] = DateTime.UtcNow;
        }
    }
}
