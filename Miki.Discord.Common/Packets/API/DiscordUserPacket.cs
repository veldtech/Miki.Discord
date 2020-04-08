using System.Runtime.Serialization;

namespace Miki.Discord.Common.Packets
{
    using System.Text.Json.Serialization;

    [DataContract]
    public class DiscordUserPacket
    {
        [JsonPropertyName("id")]
        [DataMember(Name = "id", Order = 1)]
        public ulong Id { get; set; }

        [JsonPropertyName("username")]
        [DataMember(Name = "username", Order = 2)]
        public string Username { get; set; }

        [JsonPropertyName("discriminator")]
        [DataMember(Name = "discriminator", Order = 3)]
        public string Discriminator { get; set; }

        [JsonPropertyName("bot")]
        [DataMember(Name = "bot", Order = 4)]
        public bool IsBot { get; set; }

        [JsonPropertyName("avatar")]
        [DataMember(Name = "avatar", Order = 5)]
        public string Avatar { get; set; }

        [JsonPropertyName("verified")]
        [DataMember(Name = "verified", Order = 6)]
        public bool Verified { get; set; }

        [JsonPropertyName("email")]
        [DataMember(Name = "email", Order = 7)]
        public string Email { get; set; }

        [JsonPropertyName("mfa_enabled")]
        [DataMember(Name = "mfa_enabled", Order = 8)]
        public bool MfaEnabled { get; set; }
    }
}