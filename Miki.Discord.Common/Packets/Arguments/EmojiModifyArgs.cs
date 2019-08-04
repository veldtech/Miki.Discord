using System.Runtime.Serialization;

namespace Miki.Discord.Rest
{
    [DataContract]
    public class EmojiModifyArgs
    {
        [DataMember(Name = "name")]
        public string Name { get; private set; }

        [DataMember(Name = "roles")]
        public ulong[] Roles { get; private set; }

        public EmojiModifyArgs(string name, params ulong[] roles)
        {
            Name = name;
            Roles = roles;
        }
    }
}