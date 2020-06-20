using System;

namespace Miki.Discord.Gateway
{
    /// <summary>
    /// Intents to subscribe to specific events on the Discord Gateway.
    /// <a href="https://discordapp.com/developers/docs/topics/gateway#gateway-intents">
    /// Read more here</a>
    /// </summary>
    [Flags]
    public enum GatewayIntents
    {
        /// <summary>
        /// Enables guild events for dispatch
        /// <list type="bullet">
        /// <item>GUILD_CREATE</item>
        /// <item>GUILD_DELETE</item>
        /// <item>GUILD_ROLE_CREATE</item>
        /// <item>GUILD_ROLE_UPDATE</item>
        /// <item>GUILD_ROLE_DELETE</item>
        /// <item>CHANNEL_CREATE</item>
        /// <item>CHANNEL_UPDATE</item>
        /// <item>CHANNEL_DELETE</item>
        /// <item>CHANNEL_PINS_UPDATE</item>
        /// </list>
        /// </summary>
        Guilds = 1,

        /// <summary>
        /// Enables member events for dispatch
        /// <list type="bullet">
        /// <item>GUILD_MEMBER_ADD</item>
        /// <item>GUILD_MEMBER_UPDATE</item>
        /// <item>GUILD_MEMBER_REMOVE</item>
        /// </list>
        /// </summary>
        GuildMembers = 1 << 1,

        /// <summary>
        /// Enables guild ban events for dispatch
        /// <list type="bullet">
        /// <item>GUILD_BAN_ADD</item>
        /// <item>GUILD_BAN_REMOVE</item>
        /// </list>
        /// </summary>
        GuildBans = 1 << 2,

        /// <summary>
        /// Enables guild emoji events for dispatch
        /// <list type="bullet">
        /// <item>GUILD_EMOJIS_UPDATE</item>
        /// </list>
        /// </summary>
        GuildEmojis = 1 << 3,

        /// <summary>
        /// Enables guild integration events for dispatch
        /// <list type="bullet">
        /// <item>GUILD_INTEGRATIONS_UPDATE</item>
        /// </list>
        /// </summary>
        GuildIntegrations = 1 << 4,

        /// <summary>
        /// Enables guild webhook events for dispatch
        /// <list type="bullet">
        /// <item>WEBHOOKS_UPDATE</item>
        /// </list>
        /// </summary>
        GuildWebhooks = 1 << 5,

        /// <summary>
        /// Enables guild invite events for dispatch
        /// <list type="bullet">
        /// <item>INVITE_CREATE</item>
        /// <item>INVITE_DELETE</item>
        /// </list>
        /// </summary>
        GuildInvites = 1 << 6,

        /// <summary>
        /// Enables guild voice events for dispatch
        /// <list type="bullet">
        /// <item>VOICE_STATE_UPDATE</item>
        /// </list>
        /// </summary>
        GuildVoiceStates = 1 << 7,

        /// <summary>
        /// Enables presence events for dispatch
        /// <list type="bullet">
        /// <item>PRESENCE_UPDATE</item>
        /// </list>
        /// </summary>
        GuildPresences = 1 << 8,

        /// <summary>
        /// Enables presence events for dispatch
        /// <list type="bullet">
        /// <item>MESSAGE_CREATE</item>
        /// <item>MESSAGE_UPDATE</item>
        /// <item>MESSAGE_DELETE</item>
        /// </list>
        /// </summary>
        GuildMessages = 1 << 9,

        /// <summary>
        /// Enables presence events for dispatch
        /// <list type="bullet">
        /// <item>MESSAGE_REACTION_ADD</item>
        /// <item>MESSAGE_REACTION_REMOVE</item>
        /// <item>MESSAGE_REACTION_REMOVE_ALL</item>
        /// <item>MESSAGE_REACTION_REMOVE_EMOJI</item>
        /// </list>
        /// </summary>
        GuildMessageReactions = 1 << 10,

        /// <summary>
        /// Enables presence events for dispatch
        /// <list type="bullet">
        /// <item>TYPING_START</item>
        /// </list>
        /// </summary>
        GuildMessageTyping = 1 << 11,

        /// <summary>
        /// Enables presence events for dispatch
        /// <list type="bullet">
        /// <item>CHANNEL_CREATE</item>
        /// <item>MESSAGE_CREATE</item>
        /// <item>MESSAGE_UPDATE</item>
        /// <item>MESSAGE_DELETE</item>
        /// <item>CHANNEL_PINS_UPDATE</item>
        /// </list>
        /// </summary>
        DirectMessages = 1 << 12,

        /// <summary>
        /// Enables presence events for dispatch
        /// <list type="bullet">
        /// <item>MESSAGE_REACTION_ADD</item>
        /// <item>MESSAGE_REACTION_REMOVE</item>
        /// <item>MESSAGE_REACTION_REMOVE_ALL</item>
        /// <item>MESSAGE_REACTION_REMOVE_EMOJI</item>
        /// </list>
        /// </summary>
        DirectMessageReactions = 1 << 13,

        /// <summary>
        /// Enables presence events for dispatch
        /// <list type="bullet">
        /// <item>TYPING_START</item>
        /// </list>
        /// </summary>
        DirectMessageTyping = 1 << 14,

        /// <summary>
        /// Default gateway intents if none is passed through.
        /// </summary>
        AllDefault = DirectMessageTyping 
              | DirectMessageReactions 
              | DirectMessages 
              | GuildMessageTyping 
              | GuildMessageReactions 
              | GuildMessages
              | GuildVoiceStates
              | GuildInvites
              | GuildWebhooks
              | GuildIntegrations
              | GuildEmojis
              | GuildBans
              | Guilds,

        /// <summary>
        /// All intents, includes privileged routes such as <see cref="GuildPresences"/> and
        /// <see cref="GuildMembers"/>.
        /// </summary>
        AllPrivileged = AllDefault
            | GuildPresences
            | GuildMembers

    }
}
