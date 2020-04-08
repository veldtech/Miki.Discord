using Miki.Discord.Common.Packets;
using System;
using System.Runtime.Serialization;

namespace Miki.Discord.Common
{
    using System.Text.Json.Serialization;

    [DataContract]
    public class DiscordPresence : IDiscordPresence
    {
        [JsonPropertyName("game")]
        [DataMember(Name = "game", Order = 1)]
        public DiscordActivity Activity { get; set; }

        [JsonPropertyName("status")]
        [DataMember(Name = "status", Order = 2)]
        public UserStatus Status { get; set; }

        [JsonPropertyName("since")]
        [DataMember(Name = "since", Order = 3)]
        public long Since { get; set; }

        [JsonPropertyName("afk")]
        [DataMember(Name = "afk", Order = 4)]
        public bool IsAFK { get; set; }

        public DiscordPresence()
        {}
        public DiscordPresence(DiscordPresencePacket packet)
        {
            Activity = packet.Game;
            Status = SystemUtils.ParseEnum<UserStatus>(packet.Status);
        }
    }

    public static class SystemUtils
    {
        public static T ParseEnum<T>(string enumValue, bool ignoreCase = true) where T : struct
        {
            return (T)Enum.Parse(typeof(T), enumValue, ignoreCase);
        }
    }
}