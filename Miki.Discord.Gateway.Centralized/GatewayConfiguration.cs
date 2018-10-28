using Miki.Discord.Common.Gateway;
using Miki.Net.WebSockets;

namespace Miki.Discord.Gateway.Centralized
{
	public struct GatewayConfiguration
	{
		public string Token;

		public bool Compressed;
		public GatewayEncoding Encoding;
		public int Version;

		public int ShardCount;
		public int ShardId;

		public IGatewayApiClient ApiClient;
		public IWebSocketClient WebSocketClient;

		public static GatewayConfiguration Default()
		{
			return new GatewayConfiguration
			{
				Compressed = false,
				Encoding = GatewayEncoding.Json,
				Version = GatewayConstants.DefaultVersion
			};
		}
	}
}