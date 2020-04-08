namespace Miki.Discord.Common.Gateway
{
    using System.Runtime.Serialization;

    /// <summary>
    /// Gateway bot connection recommended amount.
    /// </summary>
    public class GatewayConnectionPacket
    {
        /// <summary>
        /// Websocket URL to connect to.
        /// </summary>
        [DataMember(Name = "url")]
        public string Url;

        /// <summary>
        /// Recommended amount of shards to connect.
        /// </summary>
        [DataMember(Name = "shards")]
        public int ShardCount;

        /// <summary>
        /// Session limits.
        /// </summary>
        [DataMember(Name = "session_start_limit")]
        public GatewaySessionLimitsPacket SessionLimit;
    }
}