using Miki.Discord.Common;
using Miki.Discord.Common.Events;
using Miki.Discord.Common.Gateway;
using Miki.Discord.Common.Gateway.Packets;
using Miki.Discord.Common.Packets;
using Miki.Discord.Common.Packets.Events;
using Miki.Discord.Gateway.Connection;
using Miki.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Miki.Discord.Common.Extensions;
using Miki.Serialization;

namespace Miki.Discord.Gateway
{
	public class GatewayShard : IDisposable, IGateway
    {
        private readonly IJsonSerializer _jsonSerializer;
        private readonly GatewayConnection _connection;
		private readonly CancellationTokenSource _tokenSource;
		private bool _isRunning;

        public GatewayShard(GatewayProperties configuration)
        {
            _jsonSerializer = configuration.JsonSerializer;
			_tokenSource = new CancellationTokenSource();
			_connection = new GatewayConnection(configuration);
		}

        public int ShardId => _connection.ShardId;

        public ConnectionStatus Status => _connection.ConnectionStatus;

        public string[] TraceServers { get; private set; }

        public async Task RestartAsync()
        {
            await _connection.ReconnectAsync();
        }

		public async Task StartAsync()
		{
			if (_isRunning)
			{
				return;
			}

            _connection.Dispatch += Dispatch;

            await _connection.StartAsync();
			_isRunning = true;
		}

		public async Task StopAsync()
		{
			if (!_isRunning)
			{
				return;
			}

            _connection.Dispatch -= Dispatch;
            _tokenSource.Cancel();

			await _connection.StopAsync();

			_isRunning = false;
		}

        public Task Dispatch(GatewayMessageIdentifier identifier, byte[] data)
        {
            if (identifier.OpCode != GatewayOpcode.Dispatch)
            {
                return Task.CompletedTask;
            }

            switch (identifier.EventName)
            {
                case "MESSAGE_CREATE":
                    return Dispatch(OnMessageCreate, data);

                case "TYPING_START":
                    return Dispatch(OnTypingStart, data);

                case "PRESENCE_UPDATE":
                    return Dispatch(OnPresenceUpdate, data);

                case "MESSAGE_UPDATE":
                    return Dispatch(OnMessageUpdate, data);

                case "MESSAGE_DELETE":
                    return Dispatch(OnMessageDelete, data);

                case "GUILD_MEMBER_ADD":
                    return Dispatch(OnGuildMemberAdd, data);

                case "GUILD_MEMBER_UPDATE":
                    return Dispatch(OnGuildMemberUpdate, data);

                case "MESSAGE_DELETE_BULK":
                    return Dispatch(OnMessageDeleteBulk, data);

                case "GUILD_EMOJIS_UPDATE":
                    return Dispatch<GuildEmojisUpdateEventArgs>(packet => OnGuildEmojiUpdate(packet.GuildId, packet.Emojis), data);

                case "GUILD_MEMBER_REMOVE":
                    return Dispatch<GuildIdUserArgs>(packet => OnGuildMemberRemove(packet.GuildId, packet.User), data);

                case "GUILD_BAN_ADD":
                    return Dispatch<GuildIdUserArgs>(packet => OnGuildBanAdd(packet.GuildId, packet.User), data);

                case "GUILD_BAN_REMOVE":
                    return Dispatch<GuildIdUserArgs>(packet => OnGuildBanRemove(packet.GuildId, packet.User), data);

                case "GUILD_CREATE":
                    return Dispatch(OnGuildCreate, data);

                case "GUILD_ROLE_CREATE":
                    return Dispatch<RoleEventArgs>(packet => OnGuildRoleCreate(packet.GuildId, packet.Role), data);

                case "GUILD_ROLE_UPDATE":
                    return Dispatch<RoleEventArgs>(packet => OnGuildRoleUpdate(packet.GuildId, packet.Role), data);

                case "GUILD_ROLE_DELETE":
                    return Dispatch<RoleDeleteEventArgs>(packet => OnGuildRoleDelete(packet.GuildId, packet.RoleId), data);

                case "GUILD_UPDATE":
                    return Dispatch(OnGuildUpdate, data);

                case "GUILD_DELETE":
                    return Dispatch(OnGuildDelete, data);

                case "CHANNEL_CREATE":
                    return Dispatch(OnChannelCreate, data);

                case "CHANNEL_UPDATE":
                    return Dispatch(OnChannelUpdate, data);

                case "CHANNEL_DELETE":
                    return Dispatch(OnChannelDelete, data);

                case "USER_UPDATE":
                    return Dispatch(OnUserUpdate, data);

                case "READY":
                {
                    var packet = _jsonSerializer.Deserialize<GatewayMessage<GatewayReadyPacket>>(data).Data;
                    TraceServers = packet.TraceGuilds;
                    return OnReady.InvokeAsync(packet);
                }

                case "RESUMED":
                {
                    var packet = _jsonSerializer.Deserialize<GatewayMessage<GatewayReadyPacket>>(data).Data;
                    TraceServers = packet.TraceGuilds;
                    return OnResume.InvokeAsync(packet);
                }

                default:
                    Log.Trace($"Unhandled event {identifier.EventName}");
                    return Task.CompletedTask;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private Task Dispatch<T>(Func<T, Task> func, byte[] data)
        {
            var packet = _jsonSerializer.Deserialize<GatewayMessage<T>>(data);
            _connection.SequenceNumber = packet.SequenceNumber;
            return func.InvokeAsync(packet.Data);
        }

		public async Task SendAsync(int shardId, GatewayOpcode opcode, object payload)
		{
			if (payload == null)
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
        public Func<GatewayReadyPacket, Task> OnResume { get; set; }
        public Func<TypingStartEventArgs, Task> OnTypingStart { get; set; }
		public Func<DiscordUserPacket, Task> OnUserUpdate { get; set; }
        #endregion Events
    }
}