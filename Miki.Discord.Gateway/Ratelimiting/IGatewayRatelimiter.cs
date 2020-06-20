namespace Miki.Discord.Gateway.Ratelimiting
{
    using System.Threading;
    using System.Threading.Tasks;

    public interface IGatewayRatelimiter
    {
        Task<bool> CanIdentifyAsync(CancellationToken token);
    }
}
