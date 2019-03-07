using Miki.Discord.Common.Gateway;
using Miki.Discord.Gateway.Ratelimiting;
using Miki.Net.WebSockets;

namespace Miki.Discord.Gateway
{
    public enum GatewayMode
    {
        Default,
        RawOnly
    }

    public class GatewayProperties
    {
        /// <summary>
        /// Discord token
        /// </summary>
		public string Token;

        /// <summary>
        /// Whether the gateway should receive gzip-compressed packets.
        /// </summary>
		public bool Compressed;

        /// <summary>
        /// What kind of encoding do you want receive.
        /// </summary>
		public GatewayEncoding Encoding = GatewayEncoding.Json;

        /// <summary>
        /// If you are unsure what this should be, keep it null or GatewayConstants.DefaultVersion.
        /// </summary>
		public int Version = GatewayConstants.DefaultVersion;

        /// <summary>
        /// Total shards running on this token
        /// </summary>
		public int ShardCount = 1;

        /// <summary>
        /// The current shard's Id
        /// </summary>
		public int ShardId;

        public IWebSocketClient WebSocketClient = new BasicWebSocketClient();

        public IGatewayRatelimiter Ratelimiter = new DefaultGatewayRatelimiter();

        /// <summary>
        /// Allow events other than dispatch to be received in raw events?
        /// </summary>
        public bool AllowNonDispatchEvents = false;
	}
}