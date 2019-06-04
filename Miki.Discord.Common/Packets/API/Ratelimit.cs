using System;
using System.Runtime.Serialization;

namespace Miki.Discord.Rest
{
    /// <summary>
    /// General ratelimit struct used to verify ratelimits and block potentially ratelimited requests.
    /// </summary>
	[DataContract]
	public struct Ratelimit
	{
        /// <summary>
        /// Remaining amount of entities that can be sent on this route.
        /// </summary>
		[DataMember(Order = 1)]
        public int Remaining { get; set; }

        /// <summary>
        /// Total limit of entities that can be sent until <see cref="Reset"/> occurs.
        /// </summary>
		[DataMember(Order = 2)]
        public int Limit { get; set; }

        /// <summary>
        /// Epoch until ratelimit resets values.
        /// </summary>
		[DataMember(Order = 3)]
        public long Reset { get; set; }

        /// <summary>
        /// An optional global value for a shared ratelimit value.
        /// </summary>
		[DataMember(Order = 4)]
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