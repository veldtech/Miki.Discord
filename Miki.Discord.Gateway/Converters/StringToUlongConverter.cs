namespace Miki.Discord.Gateway.Converters
{
    using System;
    using System.Globalization;
    using System.Text.Json;
    using System.Text.Json.Serialization;

    public sealed class StringToUlongConverter : JsonConverter<ulong>
    {
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

        public override void Write(Utf8JsonWriter writer, ulong value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString(CultureInfo.InvariantCulture));
        }
    }
}