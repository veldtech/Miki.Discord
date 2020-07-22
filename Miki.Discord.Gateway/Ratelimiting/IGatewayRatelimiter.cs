using System.Threading;
using System.Threading.Tasks;

namespace Miki.Discord.Gateway.Ratelimiting
{
    /// <summary>
    /// Ratelimits identify calls on websocket.
    /// </summary>
    public interface IGatewayRatelimiter
    {
        /// <summary>
        /// Returns whether it can identify or not.
        /// </summary>
        Task<bool> CanIdentifyAsync(int shardId, CancellationToken token);
    }
}
