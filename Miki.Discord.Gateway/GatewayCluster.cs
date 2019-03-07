using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Miki.Discord.Common;
using Miki.Discord.Common.Events;
using Miki.Discord.Common.Gateway;
using Miki.Discord.Common.Gateway.Packets;
using Miki.Discord.Common.Packets;
using Miki.Discord.Common.Packets.Events;

namespace Miki.Discord.Gateway
{
    public partial class GatewayCluster : IGateway
    {
        public Dictionary<int, GatewayShard> Shards { get; set; } = new Dictionary<int, GatewayShard>();

        /// <summary>
        /// Spawn all shards in a single cluster
        /// </summary>
        /// <param name="properties">general gateway properties</param>
        /// <param name="shards">Which shards should this cluster spawn</param>
        public GatewayCluster(GatewayProperties properties)
        {
            for (int i = 0; i < properties.ShardCount; i++)
            {
                Shards.Add(i, new GatewayShard(new GatewayProperties
                {
                    WebSocketClient = properties.WebSocketClient,
                    Encoding = properties.Encoding,
                    Compressed = properties.Compressed,
                    Ratelimiter = properties.Ratelimiter,
                    ShardCount = properties.ShardCount,
                    ShardId = i,
                    Token = properties.Token,
                    Version = properties.Version
                }));
            }
        }
        /// <summary>
        /// Used to spawn specific shards only
        /// </summary>
        /// <param name="properties">general gateway properties</param>
        /// <param name="shards">Which shards should this cluster spawn</param>
        public GatewayCluster(GatewayProperties properties, IEnumerable<int> shards)
        {
            if(shards == null)
            {
                throw new ArgumentException("shards cannot be null.");
            }

            foreach(var i in shards)
            {
                Shards.Add(i, new GatewayShard(new GatewayProperties
                {
                    WebSocketClient = properties.WebSocketClient,
                    Encoding = properties.Encoding,
                    Compressed = properties.Compressed,
                    Ratelimiter = properties.Ratelimiter,
                    ShardCount = properties.ShardCount,
                    ShardId = i,
                    Token = properties.Token,
                    Version = properties.Version
                }));
            }
        }

        public async Task SendAsync(int shardId, GatewayOpcode opcode, object payload)
        {
            if(Shards.TryGetValue(shardId, out var shard))
            {
                await shard.SendAsync(shardId, opcode, payload);
            }
        }

        public async Task RestartAsync()
        {
            foreach(var shard in Shards.Values)
            {
                await shard.RestartAsync();
            }
        }

        public async Task StartAsync()
        {
            foreach(var shard in Shards.Values)
            {
                shard.OnChannelCreate += OnChannelCreate;
                shard.OnChannelDelete += OnChannelDelete;
                shard.OnChannelUpdate += OnChannelUpdate;
                shard.OnGuildBanAdd += OnGuildBanAdd;
                shard.OnGuildBanRemove += OnGuildBanRemove;
                shard.OnGuildCreate += OnGuildCreate;
                shard.OnGuildUpdate += OnGuildUpdate;
                shard.OnGuildDelete += OnGuildDelete;
                shard.OnGuildMemberAdd += OnGuildMemberAdd;
                shard.OnGuildMemberRemove += OnGuildMemberRemove;
                shard.OnGuildMemberUpdate += OnGuildMemberUpdate;
                shard.OnGuildEmojiUpdate += OnGuildEmojiUpdate;
                shard.OnGuildRoleCreate += OnGuildRoleCreate;
                shard.OnGuildRoleDelete += OnGuildRoleDelete;
                shard.OnGuildRoleDelete += OnGuildRoleDelete;
                shard.OnMessageCreate += OnMessageCreate;
                shard.OnMessageUpdate += OnMessageUpdate;
                shard.OnMessageDelete += OnMessageDelete;
                shard.OnMessageDeleteBulk += OnMessageDeleteBulk;
                shard.OnPresenceUpdate += OnPresenceUpdate;
                shard.OnReady += OnReady;
                shard.OnTypingStart += OnTypingStart;
                shard.OnUserUpdate += OnUserUpdate;
                shard.OnPacketSent += OnPacketSent;
                shard.OnPacketReceived += OnPacketReceived;
                shard.OnRawPacketReceived += OnRawPacketReceived;

                await shard.StartAsync();
            }
        }

        public async Task StopAsync()
        {
            foreach (var shard in Shards.Values)
            {
                shard.OnChannelCreate -= OnChannelCreate;
                shard.OnChannelDelete -= OnChannelDelete;
                shard.OnChannelUpdate -= OnChannelUpdate;
                shard.OnGuildBanAdd -= OnGuildBanAdd;
                shard.OnGuildBanRemove -= OnGuildBanRemove;
                shard.OnGuildCreate -= OnGuildCreate;
                shard.OnGuildUpdate -= OnGuildUpdate;
                shard.OnGuildDelete -= OnGuildDelete;
                shard.OnGuildMemberAdd -= OnGuildMemberAdd;
                shard.OnGuildMemberRemove -= OnGuildMemberRemove;
                shard.OnGuildMemberUpdate -= OnGuildMemberUpdate;
                shard.OnGuildEmojiUpdate -= OnGuildEmojiUpdate;
                shard.OnGuildRoleCreate -= OnGuildRoleCreate;
                shard.OnGuildRoleDelete -= OnGuildRoleDelete;
                shard.OnGuildRoleDelete -= OnGuildRoleDelete;
                shard.OnMessageCreate -= OnMessageCreate;
                shard.OnMessageUpdate -= OnMessageUpdate;
                shard.OnMessageDelete -= OnMessageDelete;
                shard.OnMessageDeleteBulk -= OnMessageDeleteBulk;
                shard.OnPresenceUpdate -= OnPresenceUpdate;
                shard.OnReady -= OnReady;
                shard.OnTypingStart -= OnTypingStart;
                shard.OnUserUpdate -= OnUserUpdate;
                shard.OnPacketSent -= OnPacketSent;
                shard.OnPacketReceived -= OnPacketReceived;
                shard.OnRawPacketReceived += OnRawPacketReceived;

                await shard.StopAsync();
            }
        }
    }

    public partial class GatewayCluster
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
        public event Func<GatewayMessage, Task> OnPacketReceived;
        public event Func<ArraySegment<byte>, Task> OnRawPacketReceived;

    }
}