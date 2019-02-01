using Miki.Discord.Common.Gateway;
using Miki.Net.WebSockets;

namespace Miki.Discord.Gateway.Centralized
{
    public struct GatewayProperties
    {
        /// <summary>
        /// Discord token
        /// </summary>
		public string Token;

        /// <summary>
        /// Whether the gateway should receive gzip-compressed packets.
        /// </summary>
		public bool? Compressed;

        /// <summary>
        /// What kind of encoding do you want receive.
        /// </summary>
		public GatewayEncoding? Encoding;

        /// <summary>
        /// If you are unsure what this should be, keep it null or GatewayConstants.DefaultVersion.
        /// </summary>
		public int? Version;

        /// <summary>
        /// Total shards running on this token
        /// </summary>
		public int ShardCount;

        /// <summary>
        /// The current shard's Id
        /// </summary>
		public int ShardId;

        public IWebSocketClient WebSocketClient;

        public static GatewayProperties Default()
        {
            return new GatewayProperties
            {
                Compressed = false,
                Encoding = GatewayEncoding.Json,
                Version = GatewayConstants.DefaultVersion
            };
        }
	}
}