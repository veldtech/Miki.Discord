using System.Threading.Tasks;

namespace Miki.Discord.Common.Gateway
{
    public interface IGatewayApiClient
    {
        Task<GatewayConnectionPacket> GetGatewayAsync();

        Task<GatewayConnectionPacket> GetGatewayBotAsync();
    }
}