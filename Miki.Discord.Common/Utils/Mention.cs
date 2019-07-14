using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Miki.Discord.Common.Utils
{
    public enum MentionType
    {
        NONE = 0,
        USER,
        USER_NICKNAME,
        ROLE,
        CHANNEL,
        EMOJI,
        ANIMATED_EMOJI,
    }

    public struct Mention : ISnowflake
    {
        public ulong Id { get; }
        public MentionType Type { get; }

        public Mention(ulong id, MentionType type)
        {
            Id = id;
            Type = type;
        }

        public static bool TryParse(ReadOnlySpan<char> content, out Mention value)
        {
            content = content.TrimStart('<')
                .TrimEnd('>');

            switch (content[0])
            {
                case '@':
                {
                    Mention m = ParseUserMention(content.Slice(1, content.Length - 1));
                    value = m;
                    return m.Type != MentionType.NONE;
                }

                case '#':
                {
                    if (ulong.TryParse(content.Slice(1, content.Length - 2).ToString(), out ulong result))
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
                    int lastidx = content.LastIndexOf(':');
                    if(ulong.TryParse(content.Slice(lastidx, content.Length - 1 - lastidx).ToString(), out var emojiId))
                    {
                        value = new Mention(emojiId, content[0] == 'a' 
                                ? MentionType.ANIMATED_EMOJI 
                                : MentionType.EMOJI);
                        return true;
                    }
                    value = default;
                    return false;
                }
            }
            value = default;
            return false;
        }

        private static Mention ParseUserMention(ReadOnlySpan<char> content)
        {
            if(char.IsNumber(content[0]))
            {
                if(ulong.TryParse(content.ToString(), out ulong result))
                {
                    return new Mention(result, MentionType.USER);
                }
            }
            else if(content[0] == '!')
            {
                if (ulong.TryParse(content.Slice(1, content.Length - 2).ToString(), out ulong result))
                {
                    return new Mention(result, MentionType.USER_NICKNAME);
                }
            }
            else if(content[0] == '&')
            {
                if (ulong.TryParse(content.Slice(1, content.Length - 2).ToString(), out ulong result))
                {
                    return new Mention(result, MentionType.ROLE);
                }
            }
            return default;
        }
    }
}
