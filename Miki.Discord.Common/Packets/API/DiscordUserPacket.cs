using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Miki.Discord.Common.Packets
{
    [DataContract]
    public class DiscordUserPacket
    {
        /// <summary>
        /// Internal Discord ID.
        /// </summary>
        [JsonPropertyName("id")]
        [DataMember(Name = "id", Order = 1)]
        public ulong Id { get; set; }

        /// <summary>
        /// User's name.
        /// </summary>
        [JsonPropertyName("username")]
        [DataMember(Name = "username", Order = 2)]
        public string Username { get; set; }

        /// <summary>
        /// User discriminator, aka the #1234 after someone's name.
        /// </summary>
        [JsonPropertyName("discriminator")]
        [DataMember(Name = "discriminator", Order = 3)]
        public short Discriminator { get; set; }

        /// <summary>
        /// Is the user a bot?
        /// </summary>
        [JsonPropertyName("bot")]
        [DataMember(Name = "bot", Order = 4)]
        public bool IsBot { get; set; }

        /// <summary>
        /// Avatar MD5 hash.
        /// </summary>
        [JsonPropertyName("avatar")]
        [DataMember(Name = "avatar", Order = 5)]
        public string Avatar { get; set; }

        /// <summary>
        /// User verified their phone?
        /// </summary>
        [JsonPropertyName("verified")]
        [DataMember(Name = "verified", Order = 6)]
        public bool Verified { get; set; }

        /// <summary>
        /// Email address user signed up with, only available in OAuth.
        /// </summary>
        [JsonPropertyName("email")]
        [DataMember(Name = "email", Order = 7)]
        public string Email { get; set; }

        /// <summary>
        /// Multi-factor authentication enabled.
        /// </summary>
        [JsonPropertyName("mfa_enabled")]
        [DataMember(Name = "mfa_enabled", Order = 8)]
        public bool MfaEnabled { get; set; }
    }
}