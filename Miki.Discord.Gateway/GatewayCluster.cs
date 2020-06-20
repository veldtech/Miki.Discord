namespace Miki.Discord.Gateway
{
    using Miki.Discord.Common;
    using Miki.Discord.Common.Events;
    using Miki.Discord.Common.Gateway;
    using Miki.Discord.Common.Packets;
    using Miki.Discord.Common.Packets.API;
    using Miki.Discord.Common.Packets.Events;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class GatewayCluster : IGateway
    {
        /// <inheritdoc/>
        public IGatewayEvents Events { get; }

        /// <summary>
        /// Currently running shards in this cluster.
        /// </summary>
        public Dictionary<int, IGateway> Shards { get; set; } = new Dictionary<int, IGateway>();

        /// <summary>
        /// Spawn all shards in a single cluster
        /// </summary>
        /// <param name="properties">general gateway properties</param>
        public GatewayCluster(GatewayProperties properties)
            : this(properties, Enumerable.Range(0, properties.ShardCount))
        {}

        /// <summary>
        /// Used to spawn specific shards only
        /// </summary>
        /// <param name="properties">general gateway properties</param>
        /// <param name="shards">Which shards should this cluster spawn</param>
        public GatewayCluster(GatewayProperties properties, IEnumerable<int> shards)
        {
            if(shards == null)
            {
                throw new ArgumentException("shards cannot be null.");
            }

            foreach(var i in shards)
            {
                var shardProperties = new GatewayProperties
                {
                    Encoding = properties.Encoding,
                    Compressed = properties.Compressed,
                    Ratelimiter = properties.Ratelimiter,
                    ShardCount = properties.ShardCount,
                    ShardId = i,
                    Token = properties.Token,
                    Version = properties.Version,
                    Intents = properties.Intents,
                    AllowNonDispatchEvents = properties.AllowNonDispatchEvents,
                    GatewayFactory = properties.GatewayFactory,
                    SerializerOptions = properties.SerializerOptions
                };

                Shards.Add(i, properties.GatewayFactory(shardProperties));
            }
        }

        /// <inheritdoc/>
        public async Task SendAsync(int shardId, GatewayOpcode opcode, object payload)
        {
            if(Shards.TryGetValue(shardId, out var shard))
            {
                await shard.SendAsync(shardId, opcode, payload);
            }
        }

        /// <inheritdoc/>
        public async Task RestartAsync()
        {
            foreach(var shard in Shards.Values)
            {
                await shard.RestartAsync();
            }
        }

        /// <inheritdoc/>
        public async Task StartAsync()
        {
            foreach(var shard in Shards.Values)
            {
                await shard.StartAsync()
                    .ConfigureAwait(false);
            }
        }

        /// <inheritdoc/>
        public async Task StopAsync()
        {
            foreach(var shard in Shards.Values)
            {
      
                await shard.StopAsync()
                    .ConfigureAwait(false);
            }
        }
    }
}