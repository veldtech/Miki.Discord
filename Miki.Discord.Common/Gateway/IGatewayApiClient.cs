namespace Miki.Discord.Common.Gateway
{
    using System.Threading.Tasks;

    public interface IGatewayApiClient
    {
        Task<GatewayConnectionPacket> GetGatewayAsync();

        Task<GatewayConnectionPacket> GetGatewayBotAsync();
    }
}