using Newtonsoft.Json;

namespace Miki.Discord.Gateway.Centralized
{
	public static class GatewayConstants
	{
		public const int DefaultVersion = 6;

		public const int WebSocketReceiveSize = 16 * 1024;
		public const int WebSocketSendSize = 4 * 1024;

		public static readonly JsonSerializerSettings JsonSettings
			= new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore };
	}
}