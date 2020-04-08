namespace Miki.Discord.Common.Gateway
{
    using System.Runtime.Serialization;
    using System.Text.Json.Serialization;

    /// <summary>
    /// Session limits
    /// </summary>
    [DataContract]
    public class GatewaySessionLimitsPacket
    {
        /// <summary>
        /// Total amount of reconnects for the time until <see cref="ResetAfter"/>.
        /// </summary>
        [JsonPropertyName("total")]
        [DataMember(Name = "total")]
        public int Total { get; set; }

        /// <summary>
        /// Total sum of reconnect calls available to use.
        /// </summary>
        [JsonPropertyName("remaining")]
        [DataMember(Name = "remaining")]
        public int Remaining { get; set; }

        /// <summary>
        /// Milliseconds until this session refreshes.
        /// </summary>
        [JsonPropertyName("reset_after")]
        [DataMember(Name = "reset_after")]
        public int ResetAfter { get; set; }
    }
}