using System;
using System.Globalization;
using SpanJson;
using SpanJson.Formatters;

namespace Miki.Discord.SpanJson.Formatters
{
    public sealed class LongAsStringFormatter : ICustomJsonFormatter<ulong>
    {
        public static readonly LongAsStringFormatter Default = new LongAsStringFormatter();

        public object Arguments { get; set; }

        public void Serialize(ref JsonWriter<char> writer, ulong value)
        {
            StringUtf16Formatter.Default.Serialize(ref writer, value.ToString(CultureInfo.InvariantCulture));
        }

        public ulong Deserialize(ref JsonReader<char> reader)
        {
            var value = StringUtf16Formatter.Default.Deserialize(ref reader);
            if (ulong.TryParse(value, out var longValue))
            {
                return longValue;
            }

            throw new InvalidOperationException("Invalid value.");
        }

        public void Serialize(ref JsonWriter<byte> writer, ulong value)
        {
            StringUtf8Formatter.Default.Serialize(ref writer, value.ToString(CultureInfo.InvariantCulture));
        }

        public ulong Deserialize(ref JsonReader<byte> reader)
        {
            var value = StringUtf8Formatter.Default.Deserialize(ref reader);
            if (ulong.TryParse(value, out var longValue))
            {
                return longValue;
            }

            throw new InvalidOperationException("Invalid value.");
        }
    }
}
