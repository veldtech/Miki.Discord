using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Miki.Discord.Gateway.Converters
{
    public sealed class StringToShortConverter : JsonConverter<short>
    {
        /// <inheritdoc/>
        public override short Read(
            ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var value = reader.GetString();
            if(short.TryParse(value, out var shortValue))
            {
                return shortValue;
            }

            throw new InvalidOperationException(
                $"Value '{value}' was not a valid integer.");
        }

        /// <inheritdoc/>
        public override void Write(Utf8JsonWriter writer, short value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString(CultureInfo.InvariantCulture));
        }
    }
}