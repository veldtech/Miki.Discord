using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace Miki.Discord.Gateway.WebSocket
{
    /// <summary>
    /// Default WebSocket implementation with <see cref="System.Net.WebSockets.ClientWebSocket"/>.
    /// </summary>
    public class DefaultWebSocketClient : IWebSocketClient
    {
        private readonly ClientWebSocket socket;

        /// <summary>
        /// Creates a new instance of the websocket.
        /// </summary>
        public DefaultWebSocketClient()
        {
            socket = new ClientWebSocket();
        }

        /// <inheritdoc />
        public void Dispose()
        {
            socket.Dispose();
        }

        /// <inheritdoc />
        public WebSocketCloseStatus? CloseStatus => socket.CloseStatus;

        /// <inheritdoc />
        public string CloseStatusDescription => socket.CloseStatusDescription;

        /// <inheritdoc />
        public async ValueTask CloseAsync(
            WebSocketCloseStatus closeStatus,
            string closeStatusDescription,
            CancellationToken token)
        {
            await socket.CloseOutputAsync(closeStatus, closeStatusDescription, token);
        }

        /// <inheritdoc />
        public async ValueTask ConnectAsync(Uri endpoint, CancellationToken token)
        {
            await socket.ConnectAsync(endpoint, token);
        }

        /// <inheritdoc />
        public async ValueTask<ValueWebSocketReceiveResult> ReceiveAsync(
            Memory<byte> payload, CancellationToken token)
        {
            return await socket.ReceiveAsync(payload, token);
        }

        /// <inheritdoc />
        public async ValueTask SendAsync(
            ArraySegment<byte> payload,
            WebSocketMessageType type,
            bool endOfMessage,
            CancellationToken token)
        {
            await socket.SendAsync(payload, type, endOfMessage, token);
        }
    }
}
