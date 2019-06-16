using System;
using System.Collections.Generic;
using System.Text;
using Miki.Discord.SpanJson.Formatters;
using SpanJson.Resolvers;

namespace Miki.Discord.SpanJson
{
    public sealed class DiscordResolver<T> : ResolverBase<T, DiscordResolver<T>> where T : struct
    {
        public DiscordResolver() : base(new SpanJsonOptions { EnumOption = EnumOptions.Integer })
        {
            RegisterGlobalCustomFormatter<ulong, LongAsStringFormatter>();
        }
    }
}
