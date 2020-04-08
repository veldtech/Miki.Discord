namespace Miki.Discord.Common.Gateway
{
    /// <summary>
    /// Gateway payload opcodes. Discord documentation page:
    /// <a href="https://discordapp.com/developers/docs/topics/opcodes-and-status-codes#gateway">
    /// Gateway - Opcodes and status codes.</a>
    /// </summary>
    public enum GatewayOpcode
    {
        /// <summary>
        /// Discord events being dispatched to the bot.
        /// <code>Receive only</code>
        /// </summary>
        Dispatch = 0,

        /// <summary>
        /// Opcode to send discord's gateway a heartbeat. If properly sent, the gateway returns with
        /// <see cref="GatewayOpcode.HeartbeatAcknowledge"/>. Is required to keep the connection alive.
        /// <code>Send/Receive</code>
        /// </summary>
        Heartbeat = 1,

        Identify = 2,
        
        StatusUpdate = 3,
        
        VoiceStateUpdate = 4,
        
        VoiceServerPing = 5,
        
        Resume = 6,

        /// <summary>
        /// Opcode to instruct the gateway implementation to reconnect to the gateway immediately. You
        /// are allowed to <see cref="Resume"/> afterwards.
        /// <code>Receive only</code>
        /// </summary>
        Reconnect = 7,
        
        RequestGuildMembers = 8,
        
        InvalidSession = 9,
        
        Hello = 10,

        /// <summary>
        /// Opcode for the Discord gateway to acknowledge your latest heartbeat.
        /// <code>Receive only</code>
        /// </summary>
        HeartbeatAcknowledge = 11
    }
}