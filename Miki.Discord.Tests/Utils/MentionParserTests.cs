namespace Miki.Discord.Tests.Utils
{
    using Miki.Discord.Common;
    using Xunit;

    public class MentionParserTests
    {
        [Theory]
        [InlineData("<@0>", MentionType.USER, 0, null)]
        [InlineData("<@10000000>", MentionType.USER, 10000000, null)]
        [InlineData("<@!0>", MentionType.USER_NICKNAME, 0, null)]
        [InlineData("<@!10000000>", MentionType.USER_NICKNAME, 10000000, null)]
        [InlineData("<#0>", MentionType.CHANNEL, 0, null)]
        [InlineData("<#10000000>", MentionType.CHANNEL, 10000000, null)]
        [InlineData("<@&0>", MentionType.ROLE, 0, null)]
        [InlineData("<@&10000000>", MentionType.ROLE, 10000000, null)]
        [InlineData("<:anim:0>", MentionType.EMOJI, 0, "anim")]
        [InlineData("<:anim:10000000>", MentionType.EMOJI, 10000000, "anim")]
        [InlineData("<a:anim:0>", MentionType.ANIMATED_EMOJI, 0, "anim")]
        [InlineData("<a:anim:10000000>", MentionType.ANIMATED_EMOJI, 10000000, "anim")]
        [InlineData("@everyone", MentionType.USER_ALL, 0, "everyone")]
        [InlineData("@here", MentionType.USER_ALL_ONLINE, 0, "here")]
        public void ParseValidAsync(
            string userData, 
            MentionType expectedType, 
            ulong expectedId, 
            string expectedData)
        {
            bool result = Mention.TryParse(userData, out var mention);

            Assert.True(result);
            Assert.Equal(expectedType, mention.Type);
            Assert.Equal(expectedId, mention.Id);
            Assert.Equal(expectedData, mention.Data);
        }
    }
}
