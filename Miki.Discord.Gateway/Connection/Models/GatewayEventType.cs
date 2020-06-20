namespace Miki.Discord.Common.Gateway
{
    /// <summary>
    /// All Gateway Events.
    /// </summary>
    public enum GatewayEventType
    {
        /// <summary>
        /// A new channel was created.
        /// </summary>
        ChannelCreate,

        /// <summary>
        /// A channel was deleted.
        /// </summary>
        ChannelDelete,

        /// <summary>
        /// A channel was edited.
        /// </summary>
        ChannelUpdate,
        
        /// <summary>
        /// A message was pinned on this channel.
        /// </summary>
        ChannelPinsUpdate,

        /// <summary>
        /// A member got banned.
        /// </summary>
        GuildBanAdd,

        /// <summary>
        /// A member got unbanned.
        /// </summary>
        GuildBanRemove,

        /// <summary>
        /// The bot joined a new guild.
        /// </summary>
        GuildCreate,

        /// <summary>
        /// The bot left a guild.
        /// </summary>
        GuildDelete,

        /// <summary>
        /// A new emoji got added/removed.
        /// </summary>
        GuildEmojisUpdate,
        
        /// <summary>
        /// A new integration got added/removed.
        /// </summary>
        GuildIntegrationsUpdate,

        /// <summary>
        /// A member joined a guild.
        /// </summary>
        GuildMemberAdd,

        /// <summary>
        /// A former member left a guild.
        /// </summary>
        GuildMemberRemove,

        /// <summary>
        /// A guild member updated their profile/roles/nickname.
        /// </summary>
        GuildMemberUpdate,

        /// <summary>
        /// Gateway requested members from this guild.
        /// </summary>
        GuildMembersChunk,
        GuildRoleCreate,
        GuildRoleDelete,
        GuildRoleUpdate,
        GuildUpdate,
        InviteCreate,
        InviteDelete,
        MessageCreate,
        MessageDelete,
        MessageDeleteBulk,
        MessageUpdate,
        MessageReactionAdd,
        MessageReactionRemove,
        MessageReactionRemoveAll,
        PresenceUpdate,

        /// <summary>
        /// Gateway is ready to go.
        /// </summary>
        Ready,

        /// <summary>
        /// The connection was cut and got resumed.
        /// </summary>
        Resumed,
        TypingStart,
        UserUpdate,
        VoiceServerUpdate,
        VoiceStateUpdate,
        WebhooksUpdate,

        /// <summary>
        /// Catch-all for undefined event types.
        /// </summary>
        Undefined,
    }
}