#nullable enable

using System;
using System.IO;
using System.IO.Compression;
using System.Net.WebSockets;
using System.Reactive.Subjects;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Miki.Discord.Common.Gateway;
using Miki.Discord.Gateway.Utils;
using Miki.Discord.Gateway.WebSocket;
using Miki.Logging;

namespace Miki.Discord.Gateway.Connection
{
    public enum ConnectionStatus
    {
        Connecting,
        Connected,
        Identifying,
        Resuming,
        Disconnected,
        Cancelled,
        Error
    }

    /// <summary>
    /// Basic managed connection with Discord's gateway.
    /// </summary>
    public class GatewayConnection : IHostedService
    {
        /// <summary>
        /// Event that gets called when the connection connects successfully.
        /// </summary>
        public event Func<Task> OnConnect;

        /// <summary>
        /// Event that gets called when the connection gets disconnected.
        /// </summary>
        public event Func<Exception?, Task> OnDisconnect;

        /// <summary>
        /// Event that gets called when an unexpected error gets thrown.
        /// </summary>
        public event Func<Exception, Task> OnError;

        public IObservable<GatewayMessage> OnPacketReceived => packageReceiveSubject;

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

        private readonly Subject<GatewayMessage> packageReceiveSubject;

        private ConnectionStatus lastConnectionStatus = ConnectionStatus.Disconnected;
        private ConnectionStatus connectionStatus = ConnectionStatus.Disconnected;

        private IWebSocketClient? webSocketClient;
        private readonly GatewayProperties configuration;

        private Task? runTask;
        private Task? heartbeatTask;

        private int? sequenceNumber;
        private string? sessionId;

        private readonly Memory<byte> receivePacket 
            = new byte[GatewayConstants.WebSocketReceiveSize];

        private readonly MemoryStream receiveStream = new MemoryStream();
        private readonly MemoryStream uncompressedStream = new MemoryStream();
        private readonly DeflateStream deflateStream;

        private CancellationTokenSource? connectionToken;
        private SemaphoreSlim? heartbeatLock;

        /// <summary>
        /// Shows whether the gateway is active at the moment.
        /// </summary>
        public bool IsRunning => runTask != null 
            && !(connectionToken?.IsCancellationRequested ?? true);

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
            
            deflateStream = new DeflateStream(receiveStream, CompressionMode.Decompress);
            packageReceiveSubject = new Subject<GatewayMessage>();
        }

        public async Task StartAsync(CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

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

            webSocketClient = configuration.WebSocketFactory();
            heartbeatLock = new SemaphoreSlim(0, 1);
            connectionToken = new CancellationTokenSource();

            var hello = await InitGatewayAsync(connectionToken.Token);
            TraceServers = hello.TraceServers;

            if(sequenceNumber.HasValue)
            {
                ConnectionStatus = ConnectionStatus.Resuming;
                await ResumeAsync(new GatewayResumePacket
                {
                    Sequence = sequenceNumber.Value,
                    SessionId = sessionId,
                    Token = configuration.Token
                }, connectionToken.Token);
            }
            else
            {
                ConnectionStatus = ConnectionStatus.Identifying;
                await IdentifyAsync(token);
            }

            heartbeatTask = HeartbeatAsync(hello.HeartbeatInterval, connectionToken.Token);
            runTask = RunAsync(connectionToken.Token);
        }

        private async Task IdentifyAsync(CancellationToken token)
        {
            GatewayIdentifyPacket identifyPacket = new GatewayIdentifyPacket
            {
                Compressed = configuration.Compressed,
                Token = configuration.Token,
                LargeThreshold = 250,
                Shard = new[] { configuration.ShardId, configuration.ShardCount },
                Intent = (int)configuration.Intents
            };

            while (true)
            {
                var canIdentify = await configuration.Ratelimiter
                    .CanIdentifyAsync(configuration.ShardId, token)
                    .ConfigureAwait(false);

                if (canIdentify)
                {
                    await SendCommandAsync(GatewayOpcode.Identify, identifyPacket, token)
                        .ConfigureAwait(false);
                    break;
                }

                Log.Debug("Could not identify yet, retrying in 5 seconds.");
                await Task.Delay(5000, token).ConfigureAwait(false);
            }
        }

        public async Task StopAsync(CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            
            if(connectionToken == null || runTask == null)
            {
                throw new InvalidOperationException("This gateway client is not running!");
            }

            connectionToken.Cancel();

            try
            {
                runTask.Wait();
                heartbeatTask?.Wait();
            } catch { }

            try
            {
                if (webSocketClient != null)
                {
                    await webSocketClient.CloseAsync(
                        WebSocketCloseStatus.NormalClosure,
                        string.Empty,
                        token);
                }
                ConnectionStatus = ConnectionStatus.Disconnected;
            }
            catch (TaskCanceledException)
            {
                ConnectionStatus = ConnectionStatus.Cancelled;
            }
            catch(ObjectDisposedException)
            {
                /* Means the websocket has been disposed, and is ready to be reused. */
            }
            catch(Exception ex)
            {
                ConnectionStatus = ConnectionStatus.Error;
                await ReportErrorAsync(ex);
            }

            webSocketClient?.Dispose();
            webSocketClient = null;

            connectionToken.Cancel();
            heartbeatTask = null;
            runTask = null;
        }

        private async Task RunAsync(CancellationToken token)
        {
            while(!token.IsCancellationRequested)
            {
                try
                {
                    var msg = await ReceivePacketAsync(token).ConfigureAwait(false);
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
                                heartbeatLock?.Release();
                                ConnectionStatus = ConnectionStatus.Connected;
                            }

                            if(msg.EventName == "RESUMED")
                            {
                                var readyPacket = ((JsonElement) msg.Data)
                                    .ToObject<GatewayReadyPacket>(configuration.SerializerOptions);
                                TraceServers = readyPacket.TraceGuilds;
                                heartbeatLock?.Release();
                                ConnectionStatus = ConnectionStatus.Connected;
                            }

                            packageReceiveSubject.OnNext(msg);
                            break;
                        }

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
                            await SendHeartbeatAsync(token);
                            break;
                        }

                        case GatewayOpcode.HeartbeatAcknowledge:
                        {
                            heartbeatLock?.Release();
                            break;
                        }
                    }
                }
                catch(WebSocketException w)
                {
                    await ReportErrorAsync(w);
                    _ = HandleGatewayErrorsAsync(w)
                        .ConfigureAwait(false);
                    return;
                }
                catch(TaskCanceledException)
                {
                    ConnectionStatus = ConnectionStatus.Cancelled;
                    return;
                }
                catch(Exception e)
                {
                    await ReportErrorAsync(e);
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

            if(webSocketClient == null)
            {
                throw new Exception("Websocket client already disposed before recovery was attempted");
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

        private async Task HeartbeatAsync(int latency, CancellationToken token)
        {
            // Will stop running heartbeat if connectionToken is cancelled.
            while(!token.IsCancellationRequested)
            {
                try
                {
                    if(!await heartbeatLock.WaitAsync(latency, token))
                    {
                        var _ = Task.Run(() => ReconnectAsync());
                        break;
                    }

                    await SendHeartbeatAsync(token)
                        .ConfigureAwait(false);

                    await Task.Delay(latency, token)
                        .ConfigureAwait(false);
                }
                catch(OperationCanceledException)
                {
                    break;
                }
                catch(Exception e)
                {
                    await ReportErrorAsync(e);
                    break;
                }
            }
        }

        private async Task ResumeAsync(GatewayResumePacket packet, CancellationToken token)
        {
            await SendCommandAsync(GatewayOpcode.Resume, packet, token).ConfigureAwait(false);
        }

        public async Task ReconnectAsync(
            int initialDelay = 1000,
            bool shouldIncrease = true)
        {
            var delay = initialDelay;
            bool connected = false;

            await StopAsync(default).ConfigureAwait(false);

            while(!connected)
            {
                try
                {
                    await StartAsync(default).ConfigureAwait(false);
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

        private async Task ReportErrorAsync(Exception exception)
        {
            if(OnError == null)
            {
                return;
            }

            await OnError(exception);
        }

        private async Task SendCommandAsync(GatewayMessage msg, CancellationToken token)
        {
            if(webSocketClient == null)
            {
                throw new InvalidOperationException(
                    "Cannot send command when websocket is uninitialized");
            }

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

        private async Task SendHeartbeatAsync(CancellationToken token)
        {
            GatewayMessage msg = new GatewayMessage
            {
                OpCode = GatewayOpcode.Heartbeat,
                Data = sequenceNumber
            };
            await SendCommandAsync(msg, token)
                .ConfigureAwait(false);
        }

        private async Task<GatewayHelloPacket> InitGatewayAsync(
            CancellationToken token)
        {
            string connectionUri = new WebSocketUrlBuilder("wss://gateway.discord.gg/")
                .SetCompression(configuration.Compressed)
                .SetEncoding(configuration.Encoding)
                .SetVersion(configuration.Version)
                .Build();

            if(webSocketClient == null)
            {
                throw new InvalidOperationException("Websocket uninitialized during init.");
            }
            await webSocketClient.ConnectAsync(new Uri(connectionUri), token);
            var msg = await ReceivePacketAsync(token);
            return ((JsonElement) msg.Data).ToObject<GatewayHelloPacket>(
                configuration.SerializerOptions);
        }

        private async Task ReceivePacketBytesAsync(CancellationToken token)
        {
            receiveStream.Position = 0;
            receiveStream.SetLength(0);

            ValueWebSocketReceiveResult response;
            do
            {
                token.ThrowIfCancellationRequested();

                response = await webSocketClient.ReceiveAsync(receivePacket, token)
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

                await receiveStream.WriteAsync(receivePacket, token)
                    .ConfigureAwait(false);
                receiveStream.SetLength(currentPosition + response.Count);
                receiveStream.Position = receiveStream.Length;

            } while(!response.EndOfMessage);
        }

        private async Task<GatewayMessage> ReceivePacketAsync(CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            await ReceivePacketBytesAsync(token).ConfigureAwait(false);

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

                await deflateStream.CopyToAsync(uncompressedStream, token);
                uncompressedStream.Position = 0;
            }

            if(configuration.Encoding != GatewayEncoding.Json)
            {
                return default;
            }

            return await JsonSerializer.DeserializeAsync<GatewayMessage>(
                configuration.Compressed ? uncompressedStream : receiveStream, 
                configuration.SerializerOptions,
                token);
        }
    }
}