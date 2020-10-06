using Miki.Discord.Common;
using Miki.Discord.Gateway.Connection;
using Miki.Discord.Gateway.Ratelimiting;
using System;
using System.Text.Json;
using Miki.Discord.Rest.Converters;
using Miki.Discord.Gateway.WebSocket;

namespace Miki.Discord.Gateway
{
    /// <summary>
    /// Configurable properties for the gateway client.
    /// </summary>
    public class GatewayProperties
    {
        /// <summary>
        /// Discord token
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// Whether the gateway should receive zlib-compressed packets.
        /// <code>Warning: this is not supported in this library as of now. Please check the github
        /// page.</code>
        /// </summary>
        public bool Compressed { get; set; }

        /// <summary>
        /// What kind of encoding do you want receive.
        /// </summary>
        public GatewayEncoding Encoding { get; set; } = GatewayEncoding.Json;

        /// <summary>
        /// If you are unsure what this should be, keep it null or GatewayConstants.DefaultVersion.
        /// </summary>
        public int Version { get; set; } = GatewayConstants.DefaultVersion;

        /// <summary>
        /// Total shards running on this token
        /// </summary>
        public int ShardCount { get; set; } = 1;

        /// <summary>
        /// The current shard's Id
        /// </summary>
        public int ShardId { get; set; }

        /// <summary>
        /// The gateway factory used for spawning shards in <see cref="GatewayCluster"/>.
        /// </summary>
        public Func<GatewayProperties, IGateway> GatewayFactory { get; set; } 
            = p => new GatewayShard(p);

        /// <summary>
        /// The websocket that will be used to connect to discord.
        /// </summary>
        public Func<IWebSocketClient> WebSocketFactory { get; set; }
            = () => new DefaultWebSocketClient();

        /// <summary>
        /// 
        /// </summary>
        public IGatewayRatelimiter Ratelimiter { get; set; } = new DefaultGatewayRatelimiter();

        /// <summary>
        /// Json serializer options.
        /// </summary>
        public JsonSerializerOptions SerializerOptions { get; set; } = new JsonSerializerOptions
        {
            Converters =
            {
                new StringToEnumConverter<GuildPermission>(),
                new StringToShortConverter(),
                new StringToUlongConverter()
            }
        };
         
        /// <summary>
        /// Allow events other than dispatch to be received in raw events?
        /// </summary>
        public bool AllowNonDispatchEvents { get; set; } = false;

        /// <summary>
        /// Initializes <see cref="IGatewayEvents"/> for rich events.
        /// </summary>
        public bool UseGatewayEvents { get; set; } = true;

        /// <summary>
        /// <see cref="GatewayIntents"/> to subscribe to events. If passed null, you'll subscribe to all events.
        /// </summary>
        public GatewayIntents Intents { get; set; } = GatewayIntents.AllDefault; 
    }
}