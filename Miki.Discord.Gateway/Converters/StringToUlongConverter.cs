using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Miki.Discord.Gateway.Converters
{
    public sealed class StringToUlongConverter : JsonConverter<ulong>
    {
        /// <inheritdoc/>
        public override ulong Read(
            ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var value = reader.GetString();
            if(ulong.TryParse(value, out var longValue))
            {
                return longValue;
            }

            throw new InvalidOperationException(
                $"Value '{value}' was not a valid integer.");
        }

        /// <inheritdoc/>
        public override void Write(Utf8JsonWriter writer, ulong value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString(CultureInfo.InvariantCulture));
        }
    }
}