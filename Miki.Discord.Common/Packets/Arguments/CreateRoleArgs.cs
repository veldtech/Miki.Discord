namespace Miki.Discord.Common
{
    using System.Runtime.Serialization;
    using System.Text.Json.Serialization;

    [DataContract]
    public class CreateRoleArgs
    {
        [JsonPropertyName("name")]
        [DataMember(Name = "name")]
        public string Name { get; set; }

        [JsonPropertyName("permissions")]
        [DataMember(Name = "permissions")]
        public GuildPermission? Permissions { get; set; }

        [JsonPropertyName("color")]
        [DataMember(Name = "color")]
        public int? Color { get; set; }

        [JsonPropertyName("hoist")]
        [DataMember(Name = "hoist")]
        public bool? Hoisted { get; set; }

        [JsonPropertyName("mentionable")]
        [DataMember(Name = "mentionable")]
        public bool? Mentionable { get; set; }
    }
}