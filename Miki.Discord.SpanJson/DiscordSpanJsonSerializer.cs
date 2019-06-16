using System;
using System.Collections.Generic;
using System.Text;
using Miki.Serialization.SpanJson;

namespace Miki.Discord.SpanJson
{
    public class DiscordSpanJsonSerializer : SpanJsonSerializer<DiscordResolver<char>, DiscordResolver<byte>>
    {
    }
}
