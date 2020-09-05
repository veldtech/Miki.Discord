using Miki.Discord.Common.Gateway;
using Miki.Discord.Common.Packets;
using Miki.Discord.Gateway;
using Miki.Discord.Gateway.Connection;
using Miki.Discord.Gateway.WebSocket;
using Miki.Discord.Tests.Utils;
using Moq;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace Miki.Discord.Tests.Gateway
{
    public class GatewayConnectionTests
    {
        [Fact]
        public async Task TestStateModificationAsync()
        {
            var mockWebsocketClient = new MockWebsocketClient();

            var gateway = new GatewayConnection(
                new GatewayProperties
                {
                    Token = "cannot be null",
                    WebSocketFactory = () => mockWebsocketClient,
                });

            await gateway.StartAsync(default);
            await gateway.StopAsync(default);

            Assert.False(gateway.IsRunning);
            Assert.Equal(ConnectionStatus.Disconnected, gateway.ConnectionStatus);
        }
    }
}
