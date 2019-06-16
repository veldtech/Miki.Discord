using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Miki.Discord.Rest
{
    [DataContract]
    public class DiscordRestError
    {
        [DataMember(Name = "code")]
        public int Code { get; set; }

        [DataMember(Name = "message")]
        public string Message { get; set; }

        public override string ToString()
        {
            return $"{Code}: {Message}\n";
        }
    }
}
