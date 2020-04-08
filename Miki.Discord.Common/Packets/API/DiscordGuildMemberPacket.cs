namespace Miki.Discord.Common
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Runtime.Serialization;
    using System.Text.Json.Serialization;
    using Miki.Discord.Common.Packets;

    [DataContract]
    public class DiscordGuildMemberPacket
    {
        [JsonPropertyName("user")]
        [DataMember(Name = "user", Order = 1)]
        public DiscordUserPacket User { get; set; }

        [JsonPropertyName("guild_id")]
        [DataMember(Name = "guild_id", Order = 2)]
        public ulong GuildId { get; set; }

        [JsonPropertyName("nick")]
        [DataMember(Name = "nick", Order = 3)]
        public string Nickname { get; set; }

        [JsonPropertyName("roles")]
        [DataMember(Name = "roles", Order = 4)]
        public List<ulong> Roles { get; set; } = new List<ulong>();

        [JsonPropertyName("joined_at")]
        [DataMember(Name = "joined_at", Order = 5)]
        public DateTime JoinedAt { get; set; }

        //[DataMember(Name = "joined_at")]
        //internal string joinedAt
        //{
        //    get
        //    {
        //        return new DateTime(JoinedAt).ToString("MM-dd-yyyyTHH:mm:ss.fffffffzzz", CultureInfo.InvariantCulture);
        //    }

        //    set
        //    {
        //        if(DateTime.TryParseExact(value, "MM-dd-yyyyTHH:mm:ss.fffffffzzz", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime d))
        //        {
        //            JoinedAt = d.Ticks;
        //        }

        //        if(DateTime.TryParse(value, out DateTime e))
        //        {
        //            JoinedAt = e.Ticks;
        //        }
        //    }
        //}

        [JsonPropertyName("deaf")]
        [DataMember(Name = "deaf", Order = 6)]
        public bool Deafened { get; set; }

        [JsonPropertyName("mute")]
        [DataMember(Name = "mute", Order = 7)]
        public bool Muted { get; set; }

        [JsonPropertyName("premium_since")]
        [DataMember(Name = "premium_since", Order = 8)]
        public DateTime? PremiumSince { get; set; }

        //[JsonPropertyName("premium_since")]
        //[DataMember(Name = "premium_since")]
        //internal string _premium_since
        //{
        //    get
        //    {
        //        return new DateTime(PremiumSince).ToString("MM-dd-yyyyTHH:mm:ss.fffffffzzz", CultureInfo.InvariantCulture);
        //    }

        //    set
        //    {
        //        if(DateTime.TryParseExact(value, "MM-dd-yyyyTHH:mm:ss.fffffffzzz", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime d))
        //        {
        //            PremiumSince = d.Ticks;
        //        }

        //        if(DateTime.TryParse(value, out DateTime e))
        //        {
        //            PremiumSince = e.Ticks;
        //        }
        //    }
        //}
    }
}