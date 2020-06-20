namespace Miki.Discord.Common.Packets
{
    using System.Runtime.Serialization;
    using System.Text.Json.Serialization;

    [DataContract]
    public class DiscordGuildUnavailablePacket
    {
        [JsonPropertyName("id")]
        [DataMember(Name = "id")]
        public ulong GuildId { get; set; }

        [JsonPropertyName("unavailable")]
        [DataMember(Name = "unavailable")]
        public bool? IsUnavailable { get; set; }

        /// <summary>
        /// A converter method to avoid protocol buffer serialization complexion
        /// </summary>
        /// <returns>A converted DiscordGuildPacket</returns>
        public DiscordGuildPacket ToGuildPacket()
        {
            return new DiscordGuildPacket
            {
                Id = GuildId,
                Unavailable = IsUnavailable
            };
        }
    }
}