namespace Miki.Discord.Gateway.Connection
{
    using Miki.Discord.Common.Converters;
    using Miki.Discord.Common.Gateway;
    using Miki.Discord.Common.Gateway.Packets;
    using Miki.Discord.Gateway.Utils;
    using Miki.Logging;
    using Miki.Net.WebSockets;
    using Miki.Net.WebSockets.Exceptions;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.IO.Compression;
    using System.Net.WebSockets;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    public enum ConnectionStatus
    {
        Connecting,
        Connected,
        Identifying,
        Resuming,
        Disconnecting,
        Disconnected,
        Error
    }

    public class GatewayConnection
    {
        public event Func<Task> OnConnect;
        public event Func<Exception, Task> OnDisconnect;
        public event Func<GatewayMessage, Task> OnPacketReceived;

        public ConnectionStatus ConnectionStatus { get; private set; } = ConnectionStatus.Disconnected;

        public int ShardId => configuration.ShardId;

        public string[] TraceServers { get; private set; }

        private readonly IWebSocketClient webSocketClient;
        private readonly GatewayProperties configuration;

        private Task runTask = null;
        private Task heartbeatTask = null;

        private int? sequenceNumber = null;
        private string sessionId = null;

        private readonly MemoryStream receiveStream = new MemoryStream(GatewayConstants.WebSocketReceiveSize);
        private readonly byte[] receivePacket = new byte[GatewayConstants.WebSocketReceiveSize];

        private CancellationTokenSource connectionToken;
        private SemaphoreSlim heartbeatLock;

        private readonly MemoryStream compressedStream;
        private readonly MemoryStream uncompressStream;
        private readonly StreamReader uncompressStreamReader;
        private readonly DeflateStream deflateStream;
        private readonly JsonSerializer jsonSerializer;

        public bool IsRunning => runTask != null && !connectionToken.IsCancellationRequested;

        /// <summary>
        /// Creates a new gateway connection
        /// </summary>
        /// <param name="configuration"></param>
        public GatewayConnection(GatewayProperties configuration)
        {
            if(string.IsNullOrWhiteSpace(configuration.Token))
            {
                throw new ArgumentNullException("Token cannot be empty.");
            }

            compressedStream = new MemoryStream();
            uncompressStream = new MemoryStream();
            uncompressStreamReader = new StreamReader(uncompressStream, Encoding.UTF8);
            deflateStream = new DeflateStream(compressedStream, CompressionMode.Decompress);

            jsonSerializer = JsonSerializer.Create(new
                JsonSerializerSettings
            {
                Converters = new List<JsonConverter>
                {
                    new GatewayMessageConverter()
                }
            });

            webSocketClient = configuration.WebSocketClientFactory();
            this.configuration = configuration;
        }

        public async Task StartAsync()
        {
            // Check all possible statuses before reconnecting.
            if(ConnectionStatus == ConnectionStatus.Connected
                || ConnectionStatus == ConnectionStatus.Connecting
                || ConnectionStatus == ConnectionStatus.Resuming
                || ConnectionStatus == ConnectionStatus.Identifying)
            {
                throw new InvalidOperationException("Shard has already started.");
            }

            ConnectionStatus = ConnectionStatus.Connecting;
            var hello = await InitGateway();
            TraceServers = hello.TraceServers;

            if(sequenceNumber.HasValue)
            {
                ConnectionStatus = ConnectionStatus.Resuming;
                await ResumeAsync(new GatewayResumePacket
                {
                    Sequence = sequenceNumber.Value,
                    SessionId = sessionId,
                    Token = configuration.Token
                });
            }
            else
            {
                ConnectionStatus = ConnectionStatus.Identifying;
                await IdentifyAsync();
            }

            heartbeatTask = HeartbeatAsync(hello.HeartbeatInterval);
            runTask = RunAsync();
            ConnectionStatus = ConnectionStatus.Connected;
        }

        public async Task CloseAsync()
        {
            await StopAsync();
            sessionId = null;
        }

        public async Task StopAsync()
        {
            ConnectionStatus = ConnectionStatus.Disconnecting;
            if(connectionToken == null || runTask == null)
            {
                throw new InvalidOperationException("This gateway client is not running!");
            }

            connectionToken.Cancel();

            try
            {
                runTask.Wait();
                heartbeatTask.Wait();
            }
            catch(Exception ex)
            {
                Log.Error(ex);
            }

            // Could be closed already, and will throw a WebsocketException
            try
            {
                await webSocketClient.CloseAsync(connectionToken.Token);
            }
            catch (ObjectDisposedException)
            {
                 /* Means the websocket has been disposed, and is ready to be reused. */
            }
            catch(Exception ex)
            {
                Log.Error(ex);
            }

            connectionToken = null;
            heartbeatTask = null;
            runTask = null;
            ConnectionStatus = ConnectionStatus.Disconnected;
        }

        public async Task RunAsync()
        {
            try
            {
                while(!connectionToken.IsCancellationRequested)
                {
                    var msg = await ReceivePacketAsync()
                        .ConfigureAwait(false);
                    if(!msg.OpCode.HasValue)
                    {
                        continue;
                    }

                    switch(msg.OpCode)
                    {
                        case GatewayOpcode.Dispatch:
                        {
                            sequenceNumber = msg.SequenceNumber;

                            if(msg.EventName == "READY")
                            {
                                var readyPacket = (msg.Data as JToken)?.ToObject<GatewayReadyPacket>();
                                sessionId = readyPacket.SessionId;
                                TraceServers = readyPacket.TraceGuilds;
                                heartbeatLock.Release();
                            }

                            if(msg.EventName == "RESUMED")
                            {
                                var readyPacket = (msg.Data as JToken)?.ToObject<GatewayReadyPacket>();
                                TraceServers = readyPacket.TraceGuilds;
                                heartbeatLock.Release();
                            }

                            Log.Debug($"    <= {msg.EventName.ToString()}");
                            if(OnPacketReceived != null)
                            {
                                await OnPacketReceived(msg);
                            }
                        }
                        break;

                        case GatewayOpcode.InvalidSession:
                        {
                            var canResume = (msg.Data as JToken)
                                .ToObject<bool>();
                            if(!canResume)
                            {
                                sequenceNumber = null;
                            }
                            var _ = Task.Run(() => ReconnectAsync());
                        }
                        break;

                        case GatewayOpcode.Reconnect:
                        {
                            var _ = Task.Run(() => ReconnectAsync());
                        }
                        break;

                        case GatewayOpcode.Heartbeat:
                        {
                            await SendHeartbeatAsync();
                        }
                        break;

                        case GatewayOpcode.HeartbeatAcknowledge:
                        {
                            heartbeatLock.Release();
                        }
                        break;
                    }
                }
            }
            catch(WebSocketException w)
            {
                Log.Error(w);
                _ = Task.Run(() => ReconnectAsync())
                    .ConfigureAwait(false);
            }
            catch(WebSocketCloseException c)
            {
                Log.Error(c);
                _ = HandleGatewayErrorsAsync(c)
                    .ConfigureAwait(false);
            }
            catch(TaskCanceledException)
            {
                return;
            }
            catch(Exception e)
            {
                ConnectionStatus = ConnectionStatus.Error;
                Log.Error(e);
                await Task.Delay(5000);
                _ = Task.Run(() => ReconnectAsync())
                    .ConfigureAwait(false);
            }
        }
            
        private Task HandleGatewayErrorsAsync(WebSocketCloseException w)
        {
            switch(w.ErrorCode)
            {
                default:
                {
                    Log.Warning($"Connection closed with unknown error code. ({w.ErrorCode})");
                    return Task.Run(() => ReconnectAsync());
                }

                case 4000: // unknown error
                case 4001: // unknown opcode
                case 4002: // decode error
                case 4003: // not authenticated
                case 4004: // authentication failed
                case 4005: // already authenticated
                case 4008: // rate limited
                case 4007: // invalid seq
                case 4009: // session timeout
                {
                    sequenceNumber = null;
                    return Task.Run(() => ReconnectAsync());
                }

                case 4010: // invalid shard
                case 4011: // sharding required
                {
                    return Task.Run(() => CloseAsync());
                    throw new GatewayException("Websocket returned error that should not be resumed, nor reconnected.", w);
                };
            }
        }

        public async Task HeartbeatAsync(int latency)
        {
            // Will stop running heartbeat if connectionToken is cancelled.
            while(!connectionToken.IsCancellationRequested)
            {
                try
                {
                    if(!await heartbeatLock.WaitAsync(latency, connectionToken.Token))
                    {
                        var _ = Task.Run(() => ReconnectAsync());
                        break;
                    }

                    await SendHeartbeatAsync()
                        .ConfigureAwait(false);

                    await Task.Delay(latency, connectionToken.Token)
                        .ConfigureAwait(false);
                }
                catch(OperationCanceledException)
                {
                    break;
                }
                catch(Exception e)
                {
                    Log.Error(e);
                    break;
                }
            }
        }

        public async Task IdentifyAsync()
        {
            GatewayIdentifyPacket identifyPacket = new GatewayIdentifyPacket
            {
                Compressed = configuration.Compressed,
                Token = configuration.Token,
                LargeThreshold = 250,
                Shard = new int[] { configuration.ShardId, configuration.ShardCount }
            };

            var canIdentify = await configuration.Ratelimiter.CanIdentifyAsync()
                .ConfigureAwait(false);
            while(true)
            {
                if(canIdentify)
                {
                    await SendCommandAsync(GatewayOpcode.Identify, identifyPacket, connectionToken.Token)
                        .ConfigureAwait(false);
                    break;
                }
                else
                {
                    Log.Debug("Could not identify yet, retrying in 5 seconds.");
                    await Task.Delay(5000)
                        .ConfigureAwait(false);
                    canIdentify = await configuration.Ratelimiter.CanIdentifyAsync()
                        .ConfigureAwait(false);
                }
            }
        }

        private async Task ResumeAsync(GatewayResumePacket packet)
        {
            await SendCommandAsync(GatewayOpcode.Resume, packet, connectionToken.Token)
                .ConfigureAwait(false);
        }

        public async Task ReconnectAsync(
            int initialDelay = 1000,
            bool shouldIncrease = true)
        {
            var delay = initialDelay;
            bool connected = false;

            await StopAsync()
                .ConfigureAwait(false);

            while(!connected)
            {
                try
                {
                    await StartAsync()
                        .ConfigureAwait(false);
                    connected = true;
                }
                catch(Exception e)
                {
                    ConnectionStatus = ConnectionStatus.Error;
                    Log.Error($"Reconnection failed with reason: {e.Message}, will retry in {delay / 1000} seconds");
                    await Task.Delay(delay)
                        .ConfigureAwait(false);
                    if(shouldIncrease)
                    {
                        delay += initialDelay;
                    }
                }
            }
        }

        public async Task SendCommandAsync(
            GatewayOpcode opcode,
            object data,
            CancellationToken token = default)
        {
            GatewayMessage msg = new GatewayMessage
            {
                OpCode = opcode,
                Data = data,
                EventName = null,
                SequenceNumber = null
            };
            await SendCommandAsync(msg, token)
                .ConfigureAwait(false);
        }
        private async Task SendCommandAsync(GatewayMessage msg, CancellationToken token)
        {
            var json = JsonConvert.SerializeObject(msg);

            Log.Debug($"=> {msg.OpCode.ToString()}");
            Log.Trace($"    json packet: {json}");

            await webSocketClient.SendAsync(json, token)
                .ConfigureAwait(false);
        }

        private async Task SendHeartbeatAsync()
        {
            GatewayMessage msg = new GatewayMessage
            {
                OpCode = GatewayOpcode.Heartbeat,
                Data = sequenceNumber
            };
            await SendCommandAsync(msg, connectionToken.Token)
                .ConfigureAwait(false);
        }

        private async Task<GatewayHelloPacket> InitGateway()
        {
            heartbeatLock = new SemaphoreSlim(0, 1);
            connectionToken = new CancellationTokenSource();

            string connectionUri = new WebSocketUrlBuilder("wss://gateway.discord.gg/")
                .SetCompression(configuration.Compressed)
                .SetEncoding(configuration.Encoding)
                .SetVersion(configuration.Version)
                .Build();

            await webSocketClient.ConnectAsync(new Uri(connectionUri), connectionToken.Token);
            var msg = await ReceivePacketAsync();
            return (msg.Data as JToken)
                .ToObject<GatewayHelloPacket>();
        }

        private async Task<WebSocketPacket> ReceivePacketBytesAsync()
        {
            receiveStream.Position = 0;
            receiveStream.SetLength(0);

            WebSocketResponse response;
            do
            {
                if(connectionToken.IsCancellationRequested)
                {
                    throw new OperationCanceledException();
                }

                response = await webSocketClient.ReceiveAsync(
                    new ArraySegment<byte>(receivePacket), connectionToken.Token)
                    .ConfigureAwait(false);

                if(response.Count + receiveStream.Position > receiveStream.Capacity)
                {
                    receiveStream.Capacity *= 2;
                }

                await receiveStream.WriteAsync(receivePacket, 0, response.Count, connectionToken.Token)
                    .ConfigureAwait(false);
            }
            while(!response.EndOfMessage);

            response.Count = (int)receiveStream.Position;
            Memory<byte> p = receiveStream.GetBuffer();

            return new WebSocketPacket(response, p);
        }

        private async Task<GatewayMessage> ReceivePacketAsync()
        {
            var response = await ReceivePacketBytesAsync()
                .ConfigureAwait(false);

            uncompressStream.SetLength(0);
            uncompressStream.Position = 0;

            if(configuration.Compressed)
            {
                if(response.Packet.Span[0] == 0x78)
                {
                    //Strip the zlib header
                    compressedStream.Write(response.Packet.ToArray(), 2, response.Response.Count - 2);
                    compressedStream.SetLength(response.Response.Count - 2);
                }
                else
                {
                    compressedStream.Write(response.Packet.ToArray(), 0, response.Response.Count);
                    compressedStream.SetLength(response.Response.Count);
                }

                compressedStream.Position = 0;
                await deflateStream.CopyToAsync(uncompressStream);
                compressedStream.Position = 0;
            }
            else
            {
                uncompressStream.Write(response.Packet.ToArray(), 0, response.Response.Count);
                uncompressStream.SetLength(response.Response.Count);
            }

            uncompressStream.Position = 0;

            if(configuration.Encoding == GatewayEncoding.Json)
            {
                var msg = (GatewayMessage)jsonSerializer
                    .Deserialize(uncompressStreamReader, typeof(GatewayMessage));
                return msg;
            }
            else
            {
                return new GatewayMessage { };
            }
        }
    }

    public struct GatewayPacket
    {
        public GatewayMessage Message;
        public WebSocketPacket Packet;
    }

    public struct WebSocketPacket
    {
        public WebSocketResponse Response { get; }

        public Memory<byte> Packet { get; }

        public WebSocketPacket(WebSocketResponse r, Memory<byte> packet)
        {
            Packet = packet;
            Response = r;
        }
    }
}