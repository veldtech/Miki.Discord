namespace Miki.Discord.Gateway.WebSocket
{
    using System;
    using System.Net.WebSockets;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Abstraction for websockets.
    /// </summary>
    public interface IWebSocketClient : IDisposable
    {
        /// <summary>
        /// Current reason if websocket is closed.
        /// </summary>
        WebSocketCloseStatus? CloseStatus { get; }
        
        /// <summary>
        /// Additional server-fed data sent with a close.
        /// </summary>
        string CloseStatusDescription { get; }
        
        /// <summary>
        /// Closes the WebSocket connection.
        /// </summary>
        ValueTask CloseAsync(
            WebSocketCloseStatus closeStatus, string closeStatusDescription, CancellationToken token);
        
        /// <summary>
        /// Connects a WebSocket.
        /// </summary>
        ValueTask ConnectAsync(Uri endpoint, CancellationToken token);
        
        /// <summary>
        /// Receive a buffer from the WebSocket stream.
        /// </summary>
        ValueTask<ValueWebSocketReceiveResult> ReceiveAsync(
            Memory<byte> payload,
            CancellationToken token);
        
        /// <summary>
        /// Send a message to the server.
        /// </summary>
        ValueTask SendAsync(
            ArraySegment<byte> payload,
            WebSocketMessageType type,
            bool endOfMessage,
            CancellationToken token);
    }
}
