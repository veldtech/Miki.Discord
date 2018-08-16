using Miki.Net.WebSockets;
using System;
using System.Collections.Generic;
using System.Text;

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
