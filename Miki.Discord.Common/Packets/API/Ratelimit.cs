namespace Miki.Discord.Rest
{
    using System;
    using System.Runtime.Serialization;
    using System.Text.Json.Serialization;

    /// <summary>
    /// General ratelimit struct used to verify ratelimits and block potentially ratelimited requests.
    /// </summary>
    [DataContract]
    public struct Ratelimit
    {
        /// <summary>
        /// Remaining amount of entities that can be sent on this route.
        /// </summary>
        [JsonPropertyName("remaining")]
        [DataMember(Name = "remaining", Order = 1)]
        public int Remaining { get; set; }

        /// <summary>
        /// Total limit of entities that can be sent until <see cref="Reset"/> occurs.
        /// </summary>
        [JsonPropertyName("limit")]
        [DataMember(Name = "limit", Order = 2)]
        public int Limit { get; set; }

        /// <summary>
        /// Epoch until ratelimit resets values.
        /// </summary>
        [JsonPropertyName("reset")]
        [DataMember(Name = "reset", Order = 3)]
        public long Reset { get; set; }

        /// <summary>
        /// An optional global value for a shared ratelimit value.
        /// </summary>
        [JsonPropertyName("global")]
        [DataMember(Name = "global", Order = 4)]
        public int? Global { get; set; }

        /// <summary>
        /// Checks if the current ratelimit is valid and/or is expired.
        /// </summary>
        /// <returns>Whether the current instance is being ratelimited</returns>
        public bool IsRatelimited()
            => IsRatelimited(this);

        /// <summary>
        /// Checks if the ratelimit is valid and/or is expired.
        /// </summary>
        /// <param name="rl">The instance that is being checked.</param>
        /// <returns>Whether the instance is being ratelimited</returns>
        public static bool IsRatelimited(Ratelimit rl)
        {
            return (rl.Global <= 0 || rl.Remaining <= 0)
                && DateTime.UtcNow <= DateTimeOffset.FromUnixTimeSeconds(rl.Reset);
        }
    }
}