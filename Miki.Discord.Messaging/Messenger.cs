using Miki.Discord.Common.Events;
using Miki.Discord.Common.Packets;
using Miki.Discord.Internal;
using Miki.Discord.Messaging.Sharder;
using Miki.Discord.Rest;
using Miki.Discord.Rest.Entities;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading.Tasks;

namespace Miki.Discord.Messaging
{
	public partial class MessageClient
	{
		IConnection connection;
		IModel channel;

		EventingBasicConsumer consumer;
		MessageClientConfiguration config;

		public MessageClient(MessageClientConfiguration config)
		{
			this.config = config;

			ConnectionFactory connectionFactory = new ConnectionFactory();
			connectionFactory.Uri = new Uri(config.MessengerConfigurations);

			connection = connectionFactory.CreateConnection();

			channel = connection.CreateModel();
			channel.ExchangeDeclare(config.ExchangeName, ExchangeType.Direct);
			channel.QueueDeclare(config.QueueName, false, false, false, null);
			channel.QueueBind(config.QueueName, config.ExchangeName, "*", null);
		}

		public void Start()
		{
			consumer = new EventingBasicConsumer(channel);
			consumer.Received += async (o, e) => await OnMessageAsync(o, e);
			string consumerTag = channel.BasicConsume(config.QueueName, false, consumer);
		}

		public void Stop()
		{
			consumer.Received -= async (o, e) => await OnMessageAsync(o, e);
		}

		private async Task OnMessageAsync(object ch, BasicDeliverEventArgs ea)
		{
			var payload = Encoding.UTF8.GetString(ea.Body);
			ShardPacket body = JsonConvert.DeserializeObject<ShardPacket>(payload);

			switch (body.Meta.Opcode)
			{
				case Opcode.MessageCreate:
				{
					if (MessageCreate != null)
					{
						await MessageCreate(
							body.Data.ToObject<DiscordMessagePacket>()
						);
					}
				} break;
				case Opcode.GuildCreate:
				{
					if (GuildCreate != null)
					{
						await GuildCreate(
							body.Data.ToObject<DiscordGuildPacket>()
						);
					}
				} break;
				case Opcode.ChannelCreate:
				{
					if (GuildCreate != null)
					{
						await ChannelCreate(
							body.Data.ToObject<DiscordChannelPacket>()
						);
					}
				}	break;

				case Opcode.GuildMemberRemove:
				{
					if (GuildMemberRemove != null)
					{
						var packet = body.Data.ToObject<GuildIdUserArgs>();

						await GuildMemberRemove(
							packet.guildId,
							packet.user
						);
					}
				} break;

				case Opcode.GuildMemberAdd:
				{
					DiscordGuildMemberPacket guildMember = body.Data.ToObject<DiscordGuildMemberPacket>();

					if(GuildMemberAdd != null)
					{
						await GuildMemberAdd(guildMember);
					}
				} break;

				case Opcode.GuildMemberUpdate:
				{
					GuildMemberUpdateEventArgs guildMember = body.Data.ToObject<GuildMemberUpdateEventArgs>();

					if (GuildMemberUpdate != null)
					{
						await GuildMemberUpdate(
							guildMember
						);
					}
				} break;

				case Opcode.GuildRoleCreate:
				{
					RoleEventArgs role = body.Data.ToObject<RoleEventArgs>();

					if (GuildRoleCreate != null)
					{
						await GuildRoleCreate(
							role.GuildId,
							role.Role
						);
					}
				} break;

				case Opcode.GuildRoleDelete:
				{
					if (GuildRoleDelete != null)
					{
						RoleDeleteEventArgs role = body.Data.ToObject<RoleDeleteEventArgs>();

						await GuildRoleDelete(
							role.GuildId,
							role.RoleId
						);
					}
				} break;

				case Opcode.GuildRoleUpdate:
				{
					RoleEventArgs role = body.Data.ToObject<RoleEventArgs>();

					if (GuildRoleUpdate != null)
					{
						await GuildRoleUpdate(
							role.GuildId,
							role.Role
						);
					}
				} break;

				case Opcode.ChannelDelete:
				{
					if(ChannelDelete != null)
					{
						await ChannelDelete(
							body.Data.ToObject<DiscordChannelPacket>()
						);
					}
				} break;

				case Opcode.ChannelUpdate:
				{
					if (ChannelUpdate != null)
					{
						await ChannelUpdate(
							body.Data.ToObject<DiscordChannelPacket>()
						);
					}
				} break;

				case Opcode.GuildBanAdd:
				{
					if (GuildBanAdd != null)
					{
						var packet = body.Data.ToObject<GuildIdUserArgs>();

						await GuildBanAdd(
							packet.guildId,
							packet.user
						);
					}
				} break;

				case Opcode.GuildBanRemove:
				{
					if (GuildBanRemove != null)
					{
						var packet = body.Data.ToObject<GuildIdUserArgs>();

						await GuildBanRemove(
							packet.guildId,
							packet.user
						);
					}
				} break;

				case Opcode.GuildDelete:
				{
					if (GuildDelete != null)
					{
						var packet = body.Data.ToObject<DiscordGuildUnavailablePacket>();

						await GuildDelete(
							packet
						);
					}
				} break;

				case Opcode.GuildEmojiUpdate:
				{
				} break;

				case Opcode.GuildIntegrationsUpdate:
				{
				} break;

				case Opcode.GuildMembersChunk:
				{
				} break;

				case Opcode.GuildUpdate:
				{
					if(GuildUpdate != null)
					{
						await GuildUpdate(
							body.Data.ToObject<DiscordGuildPacket>()
						);
					}
				} break;

				case Opcode.MessageDelete:
				{
					if (MessageDelete != null)
					{
						await MessageDelete(
							body.Data.ToObject<MessageDeleteArgs>()
						);
					}
				} break;

				case Opcode.MessageDeleteBulk:
				{
				} break;

				case Opcode.MessageUpdate:
				{
					if (MessageUpdate != null)
					{
						await MessageUpdate(
							body.Data.ToObject<DiscordMessagePacket>()
						);
					}
				} break;

				case Opcode.PresenceUpdate:
				{
				} break;

				case Opcode.Ready:
				{
				} break;

				case Opcode.Resumed:
				{
				} break;

				case Opcode.TypingStart:
				{
				} break;

				case Opcode.UserUpdate:
				{
					if (UserUpdate != null)
					{
						await UserUpdate(
							body.Data.ToObject<DiscordUserPacket>()
						);
					}
				} break;

				case Opcode.VoiceServerUpdate:
				{
				} break;

				case Opcode.VoiceStateUpdate:
				{
				} break;
			}

			channel.BasicAck(ea.DeliveryTag, false);
		}
	}

	// events
	public partial class MessageClient
	{
		public Func<DiscordChannelPacket, Task> ChannelCreate;
		public Func<DiscordChannelPacket, Task> ChannelUpdate;
		public Func<DiscordChannelPacket, Task> ChannelDelete;

		public Func<DiscordGuildPacket, Task> GuildCreate;
		public Func<DiscordGuildPacket, Task> GuildUpdate;
		public Func<DiscordGuildUnavailablePacket, Task> GuildDelete;

		public Func<DiscordGuildMemberPacket, Task> GuildMemberAdd;
		public Func<ulong, DiscordUserPacket, Task> GuildMemberRemove;
		public Func<GuildMemberUpdateEventArgs, Task> GuildMemberUpdate;

		public Func<ulong, DiscordUserPacket, Task> GuildBanAdd;
		public Func<ulong, DiscordUserPacket, Task> GuildBanRemove;

		public Func<ulong, DiscordRolePacket, Task> GuildRoleCreate;
		public Func<ulong, DiscordRolePacket, Task> GuildRoleUpdate;
		public Func<ulong, ulong, Task> GuildRoleDelete;

		public Func<DiscordMessagePacket, Task> MessageCreate;
		public Func<DiscordMessagePacket, Task> MessageUpdate;
		public Func<MessageDeleteArgs, Task> MessageDelete;
		public Func<DiscordMessagePacket, Task> MessageDeleteBulk;

		public Func<DiscordUserPacket, Task> UserUpdate;
	}
}
