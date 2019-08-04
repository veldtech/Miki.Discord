using Miki.Discord.Common;
using Miki.Discord.Common.Events;
using Miki.Discord.Common.Extensions;
using Miki.Discord.Common.Gateway;
using Miki.Discord.Common.Gateway.Packets;
using Miki.Discord.Common.Packets;
using Miki.Discord.Common.Packets.Events;
using Miki.Discord.Gateway.Connection;
using Miki.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Miki.Discord.Gateway
{
    public class GatewayShard : IDisposable, IGateway
    {
        private readonly GatewayConnection _connection;
        private readonly CancellationTokenSource _tokenSource;
        private bool _isRunning;

        public GatewayShard(GatewayProperties configuration)
        {
            _tokenSource = new CancellationTokenSource();
            _connection = new GatewayConnection(configuration);
        }

        public int ShardId => _connection.ShardId;

        public ConnectionStatus Status => _connection.ConnectionStatus;

        public async Task RestartAsync()
        {
            await _connection.ReconnectAsync();
        }

        public async Task StartAsync()
        {
            if(_isRunning)
            {
                return;
            }

            _connection.OnPacketReceived += OnPacketReceivedAsync;
            await _connection.StartAsync();
            _isRunning = true;
        }

        public async Task StopAsync()
        {
            if(!_isRunning)
            {
                return;
            }

            _connection.OnPacketReceived -= OnPacketReceivedAsync;
            _tokenSource.Cancel();
            await _connection.StopAsync();
            _isRunning = false;
        }

        public Task OnPacketReceivedAsync(GatewayMessage text)
        {
            if(text.OpCode != GatewayOpcode.Dispatch || !(text.Data is JToken token))
            {
                return Task.CompletedTask;
            }

            switch(text.EventName)
            {
                case "READY":
                    return OnReady.InvokeAsync(token.ToObject<GatewayReadyPacket>());

                case "GUILD_CREATE":
                    return OnGuildCreate.InvokeAsync(token.ToObject<DiscordGuildPacket>());

                case "GUILD_ROLE_UPDATE":
                    var role = token.ToObject<RoleEventArgs>();

                    return OnGuildRoleUpdate.InvokeAsync(role.GuildId, role.Role);

                case "GUILD_MEMBER_UPDATE":
                    return OnGuildMemberUpdate.InvokeAsync(token.ToObject<GuildMemberUpdateEventArgs>());

                case "GUILD_UPDATE":
                    return OnGuildUpdate.InvokeAsync(token.ToObject<DiscordGuildPacket>());

                case "GUILD_DELETE":
                    return OnGuildDelete.InvokeAsync(token.ToObject<DiscordGuildUnavailablePacket>());

                case "MESSAGE_CREATE":
                    return OnMessageCreate.InvokeAsync(token.ToObject<DiscordMessagePacket>());

                case "PRESENCE_UPDATE":
                    return OnPresenceUpdate.InvokeAsync(token.ToObject<DiscordPresencePacket>());

                case "CHANNEL_CREATE":
                    return OnChannelCreate.InvokeAsync(token.ToObject<DiscordChannelPacket>());

                case "CHANNEL_UPDATE":
                    return OnChannelUpdate.InvokeAsync(token.ToObject<DiscordChannelPacket>());

                case "CHANNEL_DELETE":
                    return OnChannelDelete.InvokeAsync(token.ToObject<DiscordChannelPacket>());

                default:
                    Log.Debug($"{text.EventName} is not implemented.");
                    return Task.CompletedTask;
            }
        }

        public async Task SendAsync(int shardId, GatewayOpcode opcode, object payload)
        {
            if(payload == null)
            {
                throw new ArgumentNullException(nameof(payload));
            }

            await _connection.SendCommandAsync(opcode, payload, _tokenSource.Token);
        }

        public void Dispose()
        {
            _tokenSource.Dispose();
        }

        #region Events
        public Func<DiscordChannelPacket, Task> OnChannelCreate { get; set; }
        public Func<DiscordChannelPacket, Task> OnChannelUpdate { get; set; }
        public Func<DiscordChannelPacket, Task> OnChannelDelete { get; set; }
        public Func<DiscordGuildPacket, Task> OnGuildCreate { get; set; }
        public Func<DiscordGuildPacket, Task> OnGuildUpdate { get; set; }
        public Func<DiscordGuildUnavailablePacket, Task> OnGuildDelete { get; set; }
        public Func<DiscordGuildMemberPacket, Task> OnGuildMemberAdd { get; set; }
        public Func<ulong, DiscordUserPacket, Task> OnGuildMemberRemove { get; set; }
        public Func<GuildMemberUpdateEventArgs, Task> OnGuildMemberUpdate { get; set; }
        public Func<ulong, DiscordUserPacket, Task> OnGuildBanAdd { get; set; }
        public Func<ulong, DiscordUserPacket, Task> OnGuildBanRemove { get; set; }
        public Func<ulong, DiscordEmoji[], Task> OnGuildEmojiUpdate { get; set; }
        public Func<ulong, DiscordRolePacket, Task> OnGuildRoleCreate { get; set; }
        public Func<ulong, DiscordRolePacket, Task> OnGuildRoleUpdate { get; set; }
        public Func<ulong, ulong, Task> OnGuildRoleDelete { get; set; }
        public Func<DiscordMessagePacket, Task> OnMessageCreate { get; set; }
        public Func<DiscordMessagePacket, Task> OnMessageUpdate { get; set; }
        public Func<MessageDeleteArgs, Task> OnMessageDelete { get; set; }
        public Func<MessageBulkDeleteEventArgs, Task> OnMessageDeleteBulk { get; set; }
        public Func<DiscordPresencePacket, Task> OnPresenceUpdate { get; set; }
        public Func<GatewayReadyPacket, Task> OnReady { get; set; }
        public Func<TypingStartEventArgs, Task> OnTypingStart { get; set; }
        public Func<DiscordPresencePacket, Task> OnUserUpdate { get; set; }
        public event Func<GatewayMessage, Task> OnPacketSent;
        public event Func<GatewayMessage, Task> OnPacketReceived
        {
            add => _connection.OnPacketReceived += value;
            remove => _connection.OnPacketReceived -= value;
        }
        #endregion Events
    }
}