using Miki.Discord.Common;
using Miki.Discord.Common.Events;
using Miki.Discord.Common.Gateway;
using Miki.Discord.Common.Gateway.Packets;
using Miki.Discord.Common.Packets;
using Miki.Discord.Rest;
using Miki.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace Miki.Discord.Gateway.Distributed
{
	public partial class DistributedGateway : IGateway
	{
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

		public Func<ulong, DiscordEmojiPacket[], Task> OnGuildEmojiUpdate { get; set; }

		public Func<ulong, DiscordRolePacket, Task> OnGuildRoleCreate { get; set; }
		public Func<ulong, DiscordRolePacket, Task> OnGuildRoleUpdate { get; set; }
		public Func<ulong, ulong, Task> OnGuildRoleDelete { get; set; }

		public Func<DiscordMessagePacket, Task> OnMessageCreate { get; set; }
		public Func<DiscordMessagePacket, Task> OnMessageUpdate { get; set; }
		public Func<MessageDeleteArgs, Task> OnMessageDelete { get; set; }
		public Func<DiscordMessagePacket, Task> OnMessageDeleteBulk { get; set; }

		public Func<DiscordPresencePacket, Task> OnPresenceUpdate { get; set; }

		public Func<GatewayReadyPacket, Task> OnReady { get; set; }

		public Func<DiscordPresencePacket, Task> OnUserUpdate { get; set; }

		public Func<GatewayMessage, Task> OnPacketSent { get; set; }
		public Func<GatewayMessage, Task> OnPacketReceived { get; set; }

		private IConnection _connection;

		private IModel _channel;
		private IModel _commandChannel;

		private EventingBasicConsumer _consumer;

		private MessageClientConfiguration _config;

		public DistributedGateway(MessageClientConfiguration config)
		{
			_config = config;

			ConnectionFactory connectionFactory = new ConnectionFactory();
			connectionFactory.Uri = config.ConnectionString;
			connectionFactory.DispatchConsumersAsync = false;

			_connection = connectionFactory.CreateConnection();

			_connection.CallbackException += (s, args) =>
			{
				Log.Error(args.Exception);
			};

			_connection.ConnectionRecoveryError += (s, args) =>
			{
				Log.Error(args.Exception);
			};

			_connection.RecoverySucceeded += (s, args) =>
			{
				Log.Debug("Rabbit Connection Recovered!");
			};

			_channel = _connection.CreateModel();
			_channel.BasicQos(config.PrefetchSize, config.PrefetchCount, false);
			_channel.ExchangeDeclare(config.ExchangeName, ExchangeType.Direct);
			_channel.QueueDeclare(config.QueueName, config.QueueDurable, config.QueueExclusive, config.QueueAutoDelete, null);
			_channel.QueueBind(config.QueueName, config.ExchangeName, config.ExchangeRoutingKey, null);

			_commandChannel = connectionFactory.CreateConnection().CreateModel();
			_commandChannel.ExchangeDeclare(config.QueueName + "-command", ExchangeType.Fanout, true);
			_commandChannel.QueueDeclare(config.QueueName + "-command", true, false, false);
			_commandChannel.QueueBind(config.QueueName + "-command", config.QueueName + "-command", config.ExchangeRoutingKey, null);
		}

		public Task StartAsync()
		{
			_consumer = new EventingBasicConsumer(_channel);
			_consumer.Received += async (ch, ea) => await OnMessageAsync(ch, ea);

			string consumerTag = _channel.BasicConsume(_config.QueueName, _config.ConsumerAutoAck, _consumer);
			return Task.CompletedTask;
		}

		public Task StopAsync()
		{
			_consumer.Received -= async (ch, ea) => await OnMessageAsync(ch, ea);
			_consumer = null;
			return Task.CompletedTask;
		}

		private async Task OnMessageAsync(object ch, BasicDeliverEventArgs ea)
		{
			var payload = Encoding.UTF8.GetString(ea.Body);
			var sw = Stopwatch.StartNew();
			GatewayMessage body = JsonConvert.DeserializeObject<GatewayMessage>(payload);

			try
			{
				Log.Trace("packet with the op-code '" + body.EventName + "' received.");

				switch (Enum.Parse(typeof(GatewayEventType), body.EventName.Replace("_", ""), true))
				{
					case GatewayEventType.MessageCreate:
					{
						if (OnMessageCreate != null)
						{
							await OnMessageCreate(
								body.Data.ToObject<DiscordMessagePacket>()
							);
						}
					}
					break;

					case GatewayEventType.GuildCreate:
					{
						if (OnGuildCreate != null)
						{
							var guild = body.Data.ToObject<DiscordGuildPacket>();

							await OnGuildCreate(
								guild
							);
						}
					}
					break;

					case GatewayEventType.ChannelCreate:
					{
						if (OnGuildCreate != null)
						{
							var channel = body.Data.ToObject<DiscordChannelPacket>();

							await OnChannelCreate(
								channel
							);
						}
					}
					break;

					case GatewayEventType.GuildMemberRemove:
					{
						if (OnGuildMemberRemove != null)
						{
							var packet = body.Data.ToObject<GuildIdUserArgs>();

							await OnGuildMemberRemove(
								packet.guildId,
								packet.user
							);
						}
					}
					break;

					case GatewayEventType.GuildMemberAdd:
					{
						DiscordGuildMemberPacket guildMember = body.Data.ToObject<DiscordGuildMemberPacket>();

						if (OnGuildMemberAdd != null)
						{
							await OnGuildMemberAdd(guildMember);
						}
					}
					break;

					case GatewayEventType.GuildMemberUpdate:
					{
						GuildMemberUpdateEventArgs guildMember = body.Data.ToObject<GuildMemberUpdateEventArgs>();

						if (OnGuildMemberUpdate != null)
						{
							await OnGuildMemberUpdate(
								guildMember
							);
						}
					}
					break;

					case GatewayEventType.GuildRoleCreate:
					{
						RoleEventArgs role = body.Data.ToObject<RoleEventArgs>();

						if (OnGuildRoleCreate != null)
						{
							await OnGuildRoleCreate(
								role.GuildId,
								role.Role
							);
						}
					}
					break;

					case GatewayEventType.GuildRoleDelete:
					{
						if (OnGuildRoleDelete != null)
						{
							RoleDeleteEventArgs role = body.Data.ToObject<RoleDeleteEventArgs>();

							await OnGuildRoleDelete(
								role.GuildId,
								role.RoleId
							);
						}
					}
					break;

					case GatewayEventType.GuildRoleUpdate:
					{
						RoleEventArgs role = body.Data.ToObject<RoleEventArgs>();

						if (OnGuildRoleUpdate != null)
						{
							await OnGuildRoleUpdate(
								role.GuildId,
								role.Role
							);
						}
					}
					break;

					case GatewayEventType.ChannelDelete:
					{
						if (OnChannelDelete != null)
						{
							await OnChannelDelete(
								body.Data.ToObject<DiscordChannelPacket>()
							);
						}
					}
					break;

					case GatewayEventType.ChannelUpdate:
					{
						if (OnChannelUpdate != null)
						{
							await OnChannelUpdate(
								body.Data.ToObject<DiscordChannelPacket>()
							);
						}
					}
					break;

					case GatewayEventType.GuildBanAdd:
					{
						if (OnGuildBanAdd != null)
						{
							var packet = body.Data.ToObject<GuildIdUserArgs>();

							await OnGuildBanAdd(
								packet.guildId,
								packet.user
							);
						}
					}
					break;

					case GatewayEventType.GuildBanRemove:
					{
						if (OnGuildBanRemove != null)
						{
							var packet = body.Data.ToObject<GuildIdUserArgs>();

							await OnGuildBanRemove(
								packet.guildId,
								packet.user
							);
						}
					}
					break;

					case GatewayEventType.GuildDelete:
					{
						if (OnGuildDelete != null)
						{
							var packet = body.Data.ToObject<DiscordGuildUnavailablePacket>();

							await OnGuildDelete(
								packet
							);
						}
					}
					break;

					case GatewayEventType.GuildEmojisUpdate:
					{
					}
					break;

					case GatewayEventType.GuildIntegrationsUpdate:
					{
					}
					break;

					case GatewayEventType.GuildMembersChunk:
					{
					}
					break;

					case GatewayEventType.GuildUpdate:
					{
						if (OnGuildUpdate != null)
						{
							await OnGuildUpdate(
								body.Data.ToObject<DiscordGuildPacket>()
							);
						}
					}
					break;

					case GatewayEventType.MessageDelete:
					{
						if (OnMessageDelete != null)
						{
							await OnMessageDelete(
								body.Data.ToObject<MessageDeleteArgs>()
							);
						}
					}
					break;

					case GatewayEventType.MessageDeleteBulk:
					{
					}
					break;

					case GatewayEventType.MessageUpdate:
					{
						if (OnMessageUpdate != null)
						{
							await OnMessageUpdate(
								body.Data.ToObject<DiscordMessagePacket>()
							);
						}
					}
					break;

					case GatewayEventType.PresenceUpdate:
					{
						if (OnPresenceUpdate != null)
						{
							await OnPresenceUpdate(
								body.Data.ToObject<DiscordPresencePacket>()
							);
						}
					}
					break;

					case GatewayEventType.Ready:
					{
						if (OnReady != null)
						{
							await OnReady(
								body.Data.ToObject<GatewayReadyPacket>()
							);
						}
					}
					break;

					case GatewayEventType.Resumed:
					{
					}
					break;

					case GatewayEventType.TypingStart:
					{
					}
					break;

					case GatewayEventType.UserUpdate:
					{
						if (OnUserUpdate != null)
						{
							await OnUserUpdate(
								body.Data.ToObject<DiscordPresencePacket>()
							);
						}
					}
					break;

					case GatewayEventType.VoiceServerUpdate:
					{
					}
					break;

					case GatewayEventType.VoiceStateUpdate:
					{
					}
					break;
				}

				if (!_config.ConsumerAutoAck)
				{
					_channel.BasicAck(ea.DeliveryTag, false);
				}
			}
			catch (Exception e)
			{
				Log.Error(e);

				if (!_config.ConsumerAutoAck)
				{
					_channel.BasicNack(ea.DeliveryTag, false, false);
				}
			}
			Log.Debug($"{body.EventName}: {sw.ElapsedMilliseconds}ms");
		}

		public Task SendAsync(int shardId, GatewayOpcode opcode, object payload)
		{
			CommandMessage msg = new CommandMessage();
			msg.Opcode = opcode;
			msg.ShardId = shardId;
			msg.Data = payload;

			_channel.BasicPublish("gateway-command", "*", body: Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(msg)));
			return Task.CompletedTask;
		}
	}
}