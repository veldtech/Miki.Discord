namespace Miki.Discord.Gateway
{
    using Miki.Discord.Common;
    using Miki.Discord.Common.Events;
    using Miki.Discord.Common.Extensions;
    using Miki.Discord.Common.Gateway;
    using Miki.Discord.Common.Packets;
    using Miki.Discord.Common.Packets.Events;
    using Miki.Discord.Gateway.Connection;
    using Miki.Logging;
    using System;
    using System.Text.Json;
    using System.Threading;
    using System.Threading.Tasks;
    using Miki.Discord.Common.Packets.API;

    public class GatewayShard : IDisposable, IGateway
    {
        private readonly GatewayProperties configuration;
        private readonly GatewayConnection connection;
        private readonly CancellationTokenSource tokenSource;
        private bool isRunning;

        public GatewayShard(GatewayProperties configuration)
        {
            this.configuration = configuration;
            tokenSource = new CancellationTokenSource();
            connection = new GatewayConnection(configuration);
        }

        public int ShardId => connection.ShardId;

        public ConnectionStatus Status => connection.ConnectionStatus;

        public async Task RestartAsync()
        {
            await connection.ReconnectAsync();
        }

        public async Task StartAsync()
        {
            if(isRunning)
            {
                return;
            }

            connection.OnPacketReceived += OnPacketReceivedAsync;
            await connection.StartAsync();
            isRunning = true;
        }

        public async Task StopAsync()
        {
            if(!isRunning)
            {
                return;
            }

            connection.OnPacketReceived -= OnPacketReceivedAsync;
            tokenSource.Cancel();
            await connection.StopAsync();
            isRunning = false;
        }

        public Task OnPacketReceivedAsync(GatewayMessage text)
        {
            if(text.OpCode != GatewayOpcode.Dispatch || !(text.Data is JsonElement elem))
            {
                return Task.CompletedTask;
            }

            switch(text.EventName)
            {
                case "CHANNEL_CREATE":
                    return OnChannelCreate.InvokeAsync(
                        elem.ToObject<DiscordChannelPacket>(configuration.SerializerOptions));

                case "CHANNEL_DELETE":
                    return OnChannelDelete.InvokeAsync(
                        elem.ToObject<DiscordChannelPacket>(configuration.SerializerOptions));

                case "CHANNEL_UPDATE":
                    return OnChannelUpdate.InvokeAsync(
                        elem.ToObject<DiscordChannelPacket>(configuration.SerializerOptions));

                case "GUILD_CREATE":
                    return OnGuildCreate.InvokeAsync(
                        elem.ToObject<DiscordGuildPacket>(configuration.SerializerOptions));

                case "GUILD_DELETE":
                    return OnGuildDelete.InvokeAsync(
                        elem.ToObject<DiscordGuildUnavailablePacket>(configuration.SerializerOptions));

                case "GUILD_MEMBER_UPDATE":
                    return OnGuildMemberUpdate.InvokeAsync(
                        elem.ToObject<GuildMemberUpdateEventArgs>(configuration.SerializerOptions));

                case "GUILD_ROLE_UPDATE":
                    var role = elem.ToObject<RoleEventArgs>(configuration.SerializerOptions);
                    return OnGuildRoleUpdate.InvokeAsync(role.GuildId, role.Role);

                case "GUILD_UPDATE":
                    return OnGuildUpdate.InvokeAsync(
                        elem.ToObject<DiscordGuildPacket>(configuration.SerializerOptions));

                case "MESSAGE_CREATE":
                    return OnMessageCreate.InvokeAsync(
                        elem.ToObject<DiscordMessagePacket>(configuration.SerializerOptions));

                case "PRESENCE_UPDATE":
                    return OnPresenceUpdate.InvokeAsync(
                        elem.ToObject<DiscordPresencePacket>(configuration.SerializerOptions));

                case "READY":
                    return OnReady.InvokeAsync(
                        elem.ToObject<GatewayReadyPacket>(configuration.SerializerOptions));

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

            await connection.SendCommandAsync(opcode, payload, tokenSource.Token);
        }

        public void Dispose()
        {
            tokenSource.Dispose();
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
            add => connection.OnPacketReceived += value;
            remove => connection.OnPacketReceived -= value;
        }
        #endregion Events
    }
}