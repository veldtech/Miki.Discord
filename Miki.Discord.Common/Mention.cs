using System;

namespace Miki.Discord.Common
{
    /// <summary>
    /// What type of mention has been parsed.
    /// </summary>
    public enum MentionType
    {
        /// <summary>
        /// Default value if nothing is parsed.
        /// </summary>
        NONE = 0,

        /// <summary>
        /// Direct user ping.
        /// </summary>
        USER,

        /// <summary>
        /// User nickname ping.
        /// </summary>
        USER_NICKNAME,

        /// <summary>
        /// Guild role ping.
        /// </summary>
        ROLE,

        /// <summary>
        /// Channel ping.
        /// </summary>
        CHANNEL,

        /// <summary>
        /// Static emoji
        /// </summary>
        EMOJI,
        
        /// <summary>
        /// Animated (gif) emoji
        /// </summary>
        ANIMATED_EMOJI,

        /// <summary>
        /// Everyone ping
        /// </summary>
        USER_ALL,

        /// <summary>
        /// Here ping
        /// </summary>
        USER_ALL_ONLINE,
    }

    /// <summary>
    /// Mention to a Discord entity.
    /// </summary>
    public struct Mention : ISnowflake
    {
        /// <summary>
        /// ID of entity.
        /// </summary>
        public ulong Id { get; }

        /// <summary>
        /// Type of entity mentioned.
        /// </summary>
        public MentionType Type { get; }

        /// <summary>
        /// Data is used for mentions that hold more info than only an <see cref="Id"/> 
        /// (e.g. <see cref="DiscordEmoji"/>).
        /// </summary>
        public string Data { get; }

        public Mention(ulong id, MentionType type, string data = null)
        {
            Id = id;
            Type = type;
            Data = data;
        }
        public static bool TryParse(ReadOnlySpan<char> content, out Mention value)
        {
            content = content.TrimStart('<')
                .TrimEnd('>');

            if (content.Length == 0)
            {
                value = default;
                return false;
            }

            switch(content[0])
            {
                case '@':
                {
                    Mention m = ParseUserMention(content[1..]);
                    value = m;
                    return m.Type != MentionType.NONE;
                }

                case '#':
                {
                    if(ulong.TryParse(
                        content[1..].ToString(), 
                        out ulong result))
                    {
                        value = new Mention(result, MentionType.CHANNEL);
                        return true;
                    }
                    value = default;
                    return false;
                }

                case 'a':
                case ':':
                {
                    int idStart = content.IndexOf(':');
                    var emojiIdStart = content.LastIndexOf(':');

                    var emojiName = content.Slice(idStart + 1, emojiIdStart - idStart - 1);

                    if(ulong.TryParse(
                        content.Slice(
                            emojiIdStart + 1, 
                            content.Length - emojiIdStart - 1).ToString(), 
                        out var emojiId))
                    {
                        var type = content[0] == 'a'
                            ? MentionType.ANIMATED_EMOJI
                            : MentionType.EMOJI;

                        value = new Mention(emojiId, type, emojiName.ToString());
                        return true;
                    }
                    value = default;
                    return false;
                }

                default:
                {
                    value = default;
                    return false;
                }
            }
        }

        private static Mention ParseUserMention(ReadOnlySpan<char> content)
        {
            if(content[0] >= '0' && content[0] <= '9')
            {
                if(ulong.TryParse(content.ToString(), out ulong result))
                {
                    return new Mention(result, MentionType.USER);
                }
            }
            else if(content[0] == '!')
            {
                if(ulong.TryParse(content[1..].ToString(), out ulong result))
                {
                    return new Mention(result, MentionType.USER_NICKNAME);
                }
            }
            else if(content[0] == '&')
            {
                if(ulong.TryParse(content[1..].ToString(), out ulong result))
                {
                    return new Mention(result, MentionType.ROLE);
                }
            }
            else if (content.SequenceEqual("everyone".AsSpan()))
            {
                return new Mention(0, MentionType.USER_ALL, "everyone");
            }
            else if(content.SequenceEqual("here".AsSpan()))
            {
                return new Mention(0, MentionType.USER_ALL_ONLINE, "here");
            }
            return default;
        }
    }
}
