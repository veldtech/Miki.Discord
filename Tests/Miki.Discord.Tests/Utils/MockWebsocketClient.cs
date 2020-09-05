using Miki.Discord.Common;
using Miki.Discord.Common.Gateway;
using Miki.Discord.Common.Packets;
using Miki.Discord.Gateway.WebSocket;
using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Miki.Discord.Tests.Utils
{
    public class MockWebsocketClient : IWebSocketClient
    {
        private bool isClosed;

        private List<byte[]> queuedMessages;
        private int currentIndex;

        public WebSocketCloseStatus? CloseStatus 
            => isClosed ? WebSocketCloseStatus.NormalClosure : (WebSocketCloseStatus?)null;

        public string CloseStatusDescription => "none";

        public MockWebsocketClient()
        {
            queuedMessages = new List<byte[]>();
        }

        public ValueTask CloseAsync(WebSocketCloseStatus closeStatus, string closeStatusDescription, CancellationToken token)
        {
            isClosed = true;
            return default;
        }

        public void PushMessage(GatewayMessage message)
        {
            var json = JsonSerializer.Serialize(message);
            queuedMessages.Add(Encoding.UTF8.GetBytes(json));
        }

        public ValueTask ConnectAsync(Uri endpoint, CancellationToken token)
        {
            isClosed = false;

            queuedMessages.Clear();

            PushMessage(
                new GatewayMessage
                {
                    Data = new GatewayHelloPacket
                    {
                        HeartbeatInterval = 2000000,
                        TraceServers = new string[0],
                    },
                });

            PushMessage(
                new GatewayMessage
                {
                    Data = new GatewayReadyPacket
                    {
                        CurrentUser = new DiscordUserPacket(),
                        Guilds = new DiscordGuildPacket[0],
                        PrivateChannels = new DiscordChannelPacket[0],
                        ProtocolVersion = 6,
                        SessionId = "lol",
                        Shard = new[] { 1, 0 },
                        TraceGuilds = new string[0]
                    }
                });

            return default;
        }

        public void Dispose()
        {
           
        }

        public async ValueTask<ValueWebSocketReceiveResult> ReceiveAsync(
            Memory<byte> payload, CancellationToken token)
        {
            while(queuedMessages.Count == 0)
            {
                token.ThrowIfCancellationRequested();
                await Task.Delay(100, token);
            }

            var msg = queuedMessages[0];
            msg.CopyTo(payload.Slice(currentIndex));

            var count = Math.Min(payload.Length, msg.Length - currentIndex);
            currentIndex += count;

            var endOfMessage = currentIndex == msg.Length;

            if(endOfMessage)
            {
                queuedMessages.RemoveAt(0);
                currentIndex = 0;
            }

            return new ValueWebSocketReceiveResult(
                count, WebSocketMessageType.Text, endOfMessage);
        }

        public ValueTask SendAsync(
            ArraySegment<byte> payload, 
            WebSocketMessageType type, 
            bool endOfMessage, 
            CancellationToken token)
        {
            return default;
        }
    }
}
