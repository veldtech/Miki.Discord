﻿using Miki.Discord.Common;
using Miki.Discord.Common.Events;
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
        public int ShardId => _connection.ShardId;
        public ConnectionStatus Status => _connection.ConnectionStatus;

        private GatewayConnection _connection;

		private CancellationTokenSource _tokenSource;
		private bool _isRunning;

		public GatewayShard(GatewayProperties configuration)
		{
			_tokenSource = new CancellationTokenSource();
			_connection = new GatewayConnection(configuration);
		}

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

			_connection.OnPacketReceived += OnPacketReceivedAsync;
			await _connection.StartAsync();
			_isRunning = true;
		}

		public async Task StopAsync()
		{
			if (!_isRunning)
			{
				return;
			}

			_connection.OnPacketReceived -= OnPacketReceivedAsync;
			_tokenSource.Cancel();
			await _connection.StopAsync();
			_isRunning = false;
		}

		public async Task OnPacketReceivedAsync(GatewayMessage text)
		{
			if (text.OpCode != GatewayOpcode.Dispatch)
			{
				return;
			}

			switch (text.EventName)
			{
                case "READY":
                {
                    if(OnReady != null)
                    {
                        await OnReady((text.Data as JToken).ToObject<GatewayReadyPacket>());
                    }
                } break;

				case "GUILD_CREATE":
				{
					if (OnGuildCreate != null)
					{
						await OnGuildCreate((text.Data as JToken).ToObject<DiscordGuildPacket>());
					}
				} break;

                case "GUILD_ROLE_UPDATE":
                {
                    if(OnGuildRoleUpdate != null)
                    {
                        var role = (text.Data as JToken).ToObject<RoleEventArgs>();
                        await OnGuildRoleUpdate(role.GuildId, role.Role);
                    }
                } break;

                case "GUILD_MEMBER_UPDATE":
                {
                    if (OnGuildRoleUpdate != null)
                    {
                        await OnGuildMemberUpdate(
                            (text.Data as JToken).ToObject<GuildMemberUpdateEventArgs>());
                    }
                }
                break;

                case "GUILD_UPDATE":
				{
					if (OnGuildUpdate != null)
					{
						await OnGuildUpdate(
                             (text.Data as JToken).ToObject<DiscordGuildPacket>());
                    }
				} break;

				case "GUILD_DELETE":
				{
					if (OnGuildDelete != null)
					{
						await OnGuildDelete(
                            (text.Data as JToken).ToObject<DiscordGuildUnavailablePacket>());
                    }
				} break;

				case "MESSAGE_CREATE":
				{
					if (OnMessageCreate != null)
					{
						await OnMessageCreate(
                            (text.Data as JToken).ToObject<DiscordMessagePacket>());
                    }
				} break;

				case "PRESENCE_UPDATE":
				{
					if (OnPresenceUpdate != null)
					{
						await OnPresenceUpdate(
                            (text.Data as JToken).ToObject<DiscordPresencePacket>());
                    }
				} break;

				case "CHANNEL_CREATE":
				{
					if (OnChannelCreate != null)
					{
						await OnChannelCreate(
                            (text.Data as JToken).ToObject<DiscordChannelPacket>());
					}
				} break;

				case "CHANNEL_UPDATE":
				{
					if (OnChannelUpdate != null)
					{
						await OnChannelUpdate(
                            (text.Data as JToken).ToObject<DiscordChannelPacket>());
                    }
				} break;

				case "CHANNEL_DELETE":
				{
					if (OnChannelDelete != null)
					{
						await OnChannelDelete((text.Data as JToken).ToObject<DiscordChannelPacket>());
                    }
				} break;

                default:
                {
                    Log.Debug($"{text.EventName} is not implemented.");
                } break;
			}
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
		public Func<TypingStartEventArgs, Task> OnTypingStart { get; set; }
		public Func<DiscordPresencePacket, Task> OnUserUpdate { get; set; }
        public event Func<GatewayMessage, Task> OnPacketSent;
        public event Func<GatewayMessage, Task> OnPacketReceived
        {
            add { _connection.OnPacketReceived += value; }
            remove { _connection.OnPacketReceived -= value; }
        }
        #endregion Events
    }
}