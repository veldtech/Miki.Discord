using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.Serialization;

namespace Miki.Discord.Common.Packets
{
    [DataContract]
    public class DiscordGuildMemberPacket
    {
        [DataMember(Name = "user", Order = 1)]
        public DiscordUserPacket User { get; set; }

        [DataMember(Name = "guild_id", Order = 2)]
        public ulong GuildId { get; set; }

        [DataMember(Name = "nick", Order = 3)]
        public string Nickname { get; set; }

        [DataMember(Name = "roles", Order = 4)]
        public List<ulong> Roles { get; set; } = new List<ulong>();

        [DataMember(Order = 5)]
        public long JoinedAt { get; set; }

        [DataMember(Name = "joined_at")]
        internal string _joinedAt
        {
            get
            {
                return new DateTime(JoinedAt).ToString("MM-dd-yyyyTHH:mm:ss.fffffffzzz", CultureInfo.InvariantCulture);
            }

            set
            {
                if(DateTime.TryParseExact(value, "MM-dd-yyyyTHH:mm:ss.fffffffzzz", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime d))
                {
                    JoinedAt = d.Ticks;
                }

                if(DateTime.TryParse(value, out DateTime e))
                {
                    JoinedAt = e.Ticks;
                }
            }
        }

        [DataMember(Name = "deaf", Order = 6)]
        public bool Deafened { get; set; }

        [DataMember(Name = "mute", Order = 7)]
        public bool Muted { get; set; }

        [DataMember(Order = 8)]
        public long PremiumSince { get; set; }

        [DataMember(Name = "premium_since")]
        internal string _premium_since
        {
            get
            {
                return new DateTime(PremiumSince).ToString("MM-dd-yyyyTHH:mm:ss.fffffffzzz", CultureInfo.InvariantCulture);
            }

            set
            {
                if(DateTime.TryParseExact(value, "MM-dd-yyyyTHH:mm:ss.fffffffzzz", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime d))
                {
                    PremiumSince = d.Ticks;
                }

                if(DateTime.TryParse(value, out DateTime e))
                {
                    PremiumSince = e.Ticks;
                }
            }
        }
    }
}