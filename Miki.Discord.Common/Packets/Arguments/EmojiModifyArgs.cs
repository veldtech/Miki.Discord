using System.Runtime.Serialization;

namespace Miki.Discord.Rest
{
    using System.Text.Json.Serialization;

    [DataContract]
    public class EmojiModifyArgs
    {
        [JsonPropertyName("name")]
        [DataMember(Name = "name")]
        public string Name { get; set; }

        [JsonPropertyName("roles")]
        [DataMember(Name = "roles")]
        public ulong[] Roles { get; set; }

        public EmojiModifyArgs(string name, params ulong[] roles)
        {
            Name = name;
            Roles = roles;
        }
    }
}