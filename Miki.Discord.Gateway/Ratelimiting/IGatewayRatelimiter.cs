using System.Threading.Tasks;

namespace Miki.Discord.Gateway.Ratelimiting
{
    using System.Threading;

    public interface IGatewayRatelimiter
    {
        Task<bool> CanIdentifyAsync(CancellationToken token);
    }
}
