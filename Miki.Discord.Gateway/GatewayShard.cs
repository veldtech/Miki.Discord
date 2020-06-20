using Miki.Discord.Common;
using Miki.Discord.Common.Gateway;
using Miki.Discord.Gateway.Connection;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Miki.Discord.Gateway
{
    public class GatewayShard : IDisposable, IGateway
    {
        private readonly GatewayConnection connection;
        private readonly GatewayEventHandler eventHandler;
        private readonly CancellationTokenSource tokenSource;
        private bool isRunning;

        /// <inheritdoc/>
        public IGatewayEvents Events => eventHandler;

        /// <summary>
        /// Index of the shard.
        /// </summary>
        public int ShardId => connection.ShardId;

        /// <summary>
        /// Current status of the connection.
        /// </summary>
        public ConnectionStatus Status => connection.ConnectionStatus;

        public GatewayShard(GatewayProperties configuration)
        {
            tokenSource = new CancellationTokenSource();
            connection = new GatewayConnection(configuration);
            eventHandler = new GatewayEventHandler(
                connection.OnPacketReceived, configuration.SerializerOptions);
        }

        /// <inheritdoc/>
        public async Task RestartAsync()
        {
            await connection.ReconnectAsync();
        }

        /// <inheritdoc/>
        public async Task StartAsync()
        {
            if(isRunning)
            {
                return;
            }

            await connection.StartAsync();
            isRunning = true;
        }

        /// <inheritdoc/>
        public async Task StopAsync()
        {
            if(!isRunning)
            {
                return;
            }

            tokenSource.Cancel();
            await connection.StopAsync();
            isRunning = false;
        }

        /// <inheritdoc/>
        public async Task SendAsync(int shardId, GatewayOpcode opcode, object payload)
        {
            if(payload == null)
            {
                throw new ArgumentNullException(nameof(payload));
            }

            await connection.SendCommandAsync(opcode, payload, tokenSource.Token);
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            tokenSource.Dispose();
        }
    }
}