using Miki.Discord.Common.Extensions;
using Miki.Discord.Common.Packets;
using Miki.Discord.Gateway;
using Miki.Discord.Tests.Dummy;
using System.Threading.Tasks;
using Xunit;

namespace Miki.Discord.Tests
{
    public class DiscordClientTest
    {
        [Fact]
        public async Task MultipleEvents()
        {
            // Create the cluster and add the events.
            var gatewayProperties = new GatewayProperties
            {
                ShardCount = 2,
                GatewayFactory = _ => new DummyGateway()
            };
            var gatewayCluster = new GatewayCluster(gatewayProperties);

            Assert.Equal(2, gatewayCluster.Shards.Count);

            var clusterEventOne = 0;
            var clusterEventTwo = 0;

            gatewayCluster.OnMessageCreate += _ =>
            {
                clusterEventOne++;
                return Task.CompletedTask;
            };

            gatewayCluster.OnMessageCreate += _ =>
            {
                clusterEventTwo++;
                return Task.CompletedTask;
            };

            // Create the DiscordClient and attach the events.
            var config = new DiscordClientConfigurations
            {
                Gateway = gatewayCluster
            };
            var client = new DiscordClient(config);

            var clientEventOne = 0;
            var clientEventTwo = 0;

            client.MessageCreate += _ =>
            {
                clientEventOne++;
                return Task.CompletedTask;
            };

            client.MessageCreate += _ =>
            {
                clientEventTwo++;
                return Task.CompletedTask;
            };

            await gatewayCluster.StartAsync();

            // Invoke the event on shard 1 and 2.
            await ((DummyGateway)gatewayCluster.Shards[0]).OnMessageCreate.InvokeAsync(new DiscordMessagePacket());

            Assert.Equal(1, clientEventOne);
            Assert.Equal(1, clientEventTwo);
            Assert.Equal(1, clusterEventOne);
            Assert.Equal(1, clusterEventTwo);

            await ((DummyGateway)gatewayCluster.Shards[1]).OnMessageCreate.InvokeAsync(new DiscordMessagePacket());

            Assert.Equal(2, clientEventOne);
            Assert.Equal(2, clientEventTwo);
            Assert.Equal(2, clusterEventOne);
            Assert.Equal(2, clusterEventTwo);
        }
    }
}
