using System;
using System.Collections.Generic;
using System.Text;

namespace Miki.Discord.Gateway.Centralized
{
    public static class GatewayConstants
    {
		public const string BaseUrl = "wss://gateway.discord.gg/";
		public const int DefaultVersion = 6;

		public const int WebSocketReceiveSize = 16 * 1024;
		public const int WebSocketSendSize = 4 * 1024;
	}
}
