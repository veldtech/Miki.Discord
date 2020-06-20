namespace Miki.Discord.Gateway.Connection
{
    using System;
    using System.IO;
    using System.IO.Compression;
    using System.Net.WebSockets;
    using System.Reactive.Subjects;
    using System.Text.Json;
    using System.Threading;
    using System.Threading.Tasks;
    using Miki.Discord.Common.Gateway;
    using Miki.Discord.Gateway.Utils;
    using Miki.Discord.Gateway.WebSocket;
    using Miki.Logging;

    public enum ConnectionStatus
    {
        Connecting,
        Connected,
        Identifying,
        Resuming,
        Disconnected,
        Error
    }

    public class GatewayConnection
    {
        public event Func<Task> OnConnect;
        public event Func<Exception, Task> OnDisconnect;

        public IObservable<GatewayMessage> OnPacketReceived => packageReceiveSubject;
        private readonly Subject<GatewayMessage> packageReceiveSubject;

        private ConnectionStatus lastConnectionStatus = ConnectionStatus.Disconnected;
        private ConnectionStatus connectionStatus = ConnectionStatus.Disconnected;

        public ConnectionStatus ConnectionStatus
        {
            get => connectionStatus;
            private set
            {
                lastConnectionStatus = connectionStatus;
                connectionStatus = value;
            }
        }

        public int ShardId => configuration.ShardId;

        public string[] TraceServers { get; private set; }

        private IWebSocketClient webSocketClient;
        private readonly GatewayProperties configuration;

        private Task runTask;
        private Task heartbeatTask;

        private int? sequenceNumber;
        private string sessionId;

        private readonly Memory<byte> receivePacket 
            = new byte[GatewayConstants.WebSocketReceiveSize];

        private readonly MemoryStream receiveStream = new MemoryStream();
        private readonly MemoryStream uncompressedStream = new MemoryStream();
        private readonly DeflateStream deflateStream;

        private CancellationTokenSource connectionToken;
        private SemaphoreSlim heartbeatLock;

        /// <summary>
        /// Shows whether the gateway is active at the moment.
        /// </summary>
        public bool IsRunning => runTask != null && !connectionToken.IsCancellationRequested;

        /// <summary>
        /// Creates a new gateway connection
        /// </summary>
        /// <param name="configuration"></param>
        public GatewayConnection(GatewayProperties configuration)
        {
            if(string.IsNullOrWhiteSpace(configuration.Token))
            {
                throw new ArgumentNullException(nameof(configuration.Token));
            }

            if(configuration.Compressed 
               || configuration.Encoding == GatewayEncoding.ETF)
            {
                throw new NotSupportedException("Compressed and ETF connections are not supported.");
            }

            this.configuration = configuration;
            this.deflateStream = new DeflateStream(receiveStream, CompressionMode.Decompress);

            packageReceiveSubject = new Subject<GatewayMessage>();
        }

        public async Task StartAsync()
        {
            // Check all possible statuses before reconnecting.
            if(ConnectionStatus == ConnectionStatus.Connected
               || ConnectionStatus == ConnectionStatus.Connecting
                || ConnectionStatus == ConnectionStatus.Identifying)
            {
                throw new InvalidOperationException("Shard has already started.");
            }

            if(lastConnectionStatus == ConnectionStatus.Resuming)
            {
                sequenceNumber = null;
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
                await IdentifyAsync(new CancellationTokenSource().Token);
            }

            heartbeatTask = HeartbeatAsync(hello.HeartbeatInterval);
            runTask = RunAsync();
        }

        public async Task CloseAsync()
        {
            await StopAsync();
            sessionId = null;
        }

        public async Task StopAsync()
        {
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

            try
            {
                await webSocketClient.CloseAsync(
                    WebSocketCloseStatus.NormalClosure, 
                    string.Empty, 
                    connectionToken.Token);
            }
            catch(ObjectDisposedException)
            {
                /* Means the websocket has been disposed, and is ready to be reused. */
            }
            catch(Exception ex)
            {
                Log.Error(ex);
            }

            webSocketClient.Dispose();
            webSocketClient = null;

            connectionToken = null;
            heartbeatTask = null;
            runTask = null;
            ConnectionStatus = ConnectionStatus.Disconnected;
        }

        private async Task RunAsync()
        {
            while(!connectionToken.IsCancellationRequested)
            {
                try
                {
                    var msg = await ReceivePacketAsync().ConfigureAwait(false);
                    if(!msg.OpCode.HasValue)
                    {
                        continue;
                    }

                    switch(msg.OpCode)
                    {
                        case GatewayOpcode.Dispatch:
                        {
                            Log.Debug("<= " + msg.EventName);

                            sequenceNumber = msg.SequenceNumber;

                            if(msg.EventName == "READY")
                            {
                                var readyPacket = ((JsonElement) msg.Data)
                                    .ToObject<GatewayReadyPacket>(configuration.SerializerOptions);
                                sessionId = readyPacket.SessionId;
                                TraceServers = readyPacket.TraceGuilds;
                                heartbeatLock.Release();
                                ConnectionStatus = ConnectionStatus.Connected;
                            }

                            if(msg.EventName == "RESUMED")
                            {
                                var readyPacket = ((JsonElement) msg.Data)
                                    .ToObject<GatewayReadyPacket>(configuration.SerializerOptions);
                                TraceServers = readyPacket.TraceGuilds;
                                heartbeatLock.Release();
                                ConnectionStatus = ConnectionStatus.Connected;
                            }

                            packageReceiveSubject.OnNext(msg);
                        }
                            break;

                        case GatewayOpcode.InvalidSession:
                        {
                            var canResume = ((JsonElement) msg.Data).GetBoolean();
                            if(!canResume)
                            {
                                sequenceNumber = null;
                            }

                            var _ = Task.Run(() => ReconnectAsync());
                            break;
                        }

                        case GatewayOpcode.Reconnect:
                        {
                            var _ = Task.Run(() => ReconnectAsync());
                            break;
                        }

                        case GatewayOpcode.Heartbeat:
                        {
                            await SendHeartbeatAsync();
                            break;
                        }

                        case GatewayOpcode.HeartbeatAcknowledge:
                        {
                            heartbeatLock.Release();
                            break;
                        }
                    }
                }
                catch(WebSocketException w)
                {
                    Log.Error(w);
                    _ = HandleGatewayErrorsAsync(w)
                        .ConfigureAwait(false);
                    return;
                }
                catch(TaskCanceledException)
                {
                    return;
                }
                catch(Exception e)
                {
                    Log.Error(e);
                }
            }
        }

        private Task HandleGatewayErrorsAsync(WebSocketException w)
        {
            if(!CanRecoverFrom(w))
            {
                sequenceNumber = null;
            }

            return Task.Run(() => ReconnectAsync());
        }

        private bool CanRecoverFrom(WebSocketException ex)
        {
            if(ex.WebSocketErrorCode == WebSocketError.ConnectionClosedPrematurely)
            {
                return true;
            }

            if(webSocketClient.CloseStatus != null
               && (webSocketClient.CloseStatus.Value == WebSocketCloseStatus.Empty
               || webSocketClient.CloseStatus.Value == WebSocketCloseStatus.NormalClosure
               || webSocketClient.CloseStatus.Value == WebSocketCloseStatus.InternalServerError
               || webSocketClient.CloseStatus.Value == WebSocketCloseStatus.ProtocolError))
            {
                return true;
            }

            switch(ex.ErrorCode)
            {
                case 1011: // server-side error
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
                    return true;
                }

                case 4010: // invalid shard
                case 4011: // sharding required
                {
                    throw new GatewayException(
                        "Invalid configuration data caused the websocket to close irrecoverably.", ex);
                }
            }
            return false;
        }

        private async Task HeartbeatAsync(int latency)
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

        public async Task IdentifyAsync(CancellationToken token)
        {
            GatewayIdentifyPacket identifyPacket = new GatewayIdentifyPacket
            {
                Compressed = configuration.Compressed,
                Token = configuration.Token,
                LargeThreshold = 250,
                Shard = new[] {configuration.ShardId, configuration.ShardCount},
                Intent = (int)configuration.Intents
            };

            var canIdentify = await configuration.Ratelimiter.CanIdentifyAsync(token)
                .ConfigureAwait(false);
            while(true)
            {
                if(canIdentify)
                {
                    await SendCommandAsync(GatewayOpcode.Identify, identifyPacket, connectionToken.Token)
                        .ConfigureAwait(false);
                    break;
                }

                Log.Debug("Could not identify yet, retrying in 5 seconds.");
                await Task.Delay(5000, token)
                    .ConfigureAwait(false);
                canIdentify = await configuration.Ratelimiter.CanIdentifyAsync(token)
                    .ConfigureAwait(false);
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

            await StopAsync().ConfigureAwait(false);

            while(!connected)
            {
                try
                {
                    await StartAsync().ConfigureAwait(false);
                    connected = true;
                }
                catch(Exception e)
                {
                    ConnectionStatus = ConnectionStatus.Error;
                    Log.Error(
                        $"Reconnection failed with reason: {e.Message}, will retry in {delay / 1000} seconds");
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
            GatewayOpcode opcode, object data, CancellationToken token = default)
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
            await using var stream = new MemoryStream();
            await JsonSerializer.SerializeAsync(
                stream, msg, typeof(GatewayMessage), configuration.SerializerOptions, token);

            Log.Debug($"=> {msg.OpCode}");

            if(!stream.TryGetBuffer(out var buffer))
            {
                Log.Warning($"Message with opcode '{msg.OpCode}' did not send.");
                return;
            }

            await webSocketClient.SendAsync(buffer, WebSocketMessageType.Text, true, token)
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
            webSocketClient = configuration.WebSocketFactory();
            heartbeatLock = new SemaphoreSlim(0, 1);
            connectionToken = new CancellationTokenSource();

            string connectionUri = new WebSocketUrlBuilder("wss://gateway.discord.gg/")
                .SetCompression(configuration.Compressed)
                .SetEncoding(configuration.Encoding)
                .SetVersion(configuration.Version)
                .Build();

            await webSocketClient.ConnectAsync(new Uri(connectionUri), connectionToken.Token);
            var msg = await ReceivePacketAsync();
            return ((JsonElement) msg.Data).ToObject<GatewayHelloPacket>(
                configuration.SerializerOptions);
        }

        private async Task ReceivePacketBytesAsync()
        {
            receiveStream.Position = 0;
            receiveStream.SetLength(0);

            ValueWebSocketReceiveResult response;
            do
            {
                if(connectionToken.IsCancellationRequested)
                {
                    throw new OperationCanceledException();
                }

                response = await webSocketClient.ReceiveAsync(receivePacket, connectionToken.Token)
                    .ConfigureAwait(false);
                if(response.MessageType == WebSocketMessageType.Close)
                {
                    throw new WebSocketException(
                        (int)(webSocketClient.CloseStatus ?? WebSocketCloseStatus.Empty),
                        webSocketClient.CloseStatusDescription);
                }

                if(response.Count + receiveStream.Position > receiveStream.Capacity)
                {
                    receiveStream.Capacity += GatewayConstants.WebSocketReceiveSize;
                }

                var currentPosition = receiveStream.Position;

                await receiveStream.WriteAsync(receivePacket)
                    .ConfigureAwait(false);
                receiveStream.SetLength(currentPosition + response.Count);
                receiveStream.Position = receiveStream.Length;

            } while(!response.EndOfMessage);
        }

        private async Task<GatewayMessage> ReceivePacketAsync()
        {
            await ReceivePacketBytesAsync().ConfigureAwait(false);

            uncompressedStream.Position = 0;
            uncompressedStream.SetLength(0);

            receiveStream.Position = 0;

            if(configuration.Compressed)
            {
                int header = receiveStream.ReadByte();
                if(header == 0x78) // If header is zlib header, strip header.
                {
                    receiveStream.Position = 2;
                    receiveStream.SetLength(receiveStream.Length - 2);
                }
                else
                {
                    receiveStream.Position = 0;
                }

                await deflateStream.CopyToAsync(uncompressedStream);
                uncompressedStream.Position = 0;
            }

            if(configuration.Encoding != GatewayEncoding.Json)
            {
                return default;
            }

            return await JsonSerializer.DeserializeAsync<GatewayMessage>(
                configuration.Compressed ? uncompressedStream : receiveStream, 
                configuration.SerializerOptions);
        }
    }
}