using System;
using Miki.Discord.Common.Gateway;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace Miki.Discord.Common
{
    public interface IGateway : IHostedService
    {
        IObservable<GatewayMessage> PacketReceived { get; }

        IGatewayEvents Events { get; }

        Task RestartAsync();

        Task SendAsync(int shardId, GatewayOpcode opcode, object payload);
    }
}