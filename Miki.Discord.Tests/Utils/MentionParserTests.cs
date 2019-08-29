using Miki.Discord.Common.Utils;

namespace Miki.Discord.Tests.Utils
{
    using Xunit;

    public class MentionParserTests
    {
        [Theory]
        [InlineData("<@0>", MentionType.USER, 0)]
        [InlineData("<@10000000>", MentionType.USER, 10000000)]
        [InlineData("<@!0>", MentionType.USER_NICKNAME, 0)]
        [InlineData("<@!10000000>", MentionType.USER_NICKNAME, 10000000)]
        [InlineData("<#0>", MentionType.CHANNEL, 0)]
        [InlineData("<#10000000>", MentionType.CHANNEL, 10000000)]
        [InlineData("<@&0>", MentionType.ROLE, 0)]
        [InlineData("<@&10000000>", MentionType.ROLE, 10000000)]
        [InlineData("<:anim:0>", MentionType.EMOJI, 0)]
        [InlineData("<:anim:10000000>", MentionType.EMOJI, 10000000)]
        [InlineData("<a:anim:0>", MentionType.ANIMATED_EMOJI, 0)]
        [InlineData("<a:anim:10000000>", MentionType.ANIMATED_EMOJI, 10000000)]
        public void ParseValidAsync(string userData, MentionType expectedType, ulong expectedId)
        {
            Mention.TryParse(userData, out var mention);

            Assert.Equal(expectedType, mention.Type);
            Assert.Equal(expectedId, mention.Id);
        }
    }
}
