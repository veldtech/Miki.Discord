namespace Miki.Discord.Gateway.Connection
{
    public static class GatewayConstants
    {
        /// <summary>
        /// Default Gateway version supported by Miki.Discord.
        /// </summary>
        public const int DefaultVersion = 8;

        /// <summary>
        /// Default websocket receive payload size.
        /// </summary>
        public const int WebSocketReceiveSize = 16 * 1024;

        /// <summary>
        /// Default websocket send payload size.
        /// </summary>
        public const int WebSocketSendSize = 4 * 1024;
    }
}