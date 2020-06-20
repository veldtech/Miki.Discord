namespace Miki.Discord.Common
{
    using Miki.Discord.Common.Gateway;
    using System.Threading.Tasks;

    public interface IGateway
    {
        IGatewayEvents Events { get; }

        Task RestartAsync();

        Task SendAsync(int shardId, GatewayOpcode opcode, object payload);

        Task StartAsync();

        Task StopAsync();
    }
}