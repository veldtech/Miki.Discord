using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Miki.Discord.Gateway.Converters
{
    public class StringToEnumConverter<T> : JsonConverter<T> where T : Enum
    {
        public StringToEnumConverter()
        {
            if(!typeof(T).IsEnum)
            {
                throw new InvalidOperationException(
                    "Cannot use non-enum types in StringToEnumConverter");
            }
        }

        /// <inheritdoc/>
        public override T Read(
            ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var value = reader.GetString();
            if(Enum.TryParse(typeof(T), value, out var enumValue))
            {
                return (T)enumValue;
            }

            throw new InvalidOperationException(
                $"Value '{value}' was not a valid short.");
        }

        /// <inheritdoc/>
        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            var numericValue = (long)(object)value;
            writer.WriteStringValue(numericValue.ToString());
        }
    }
}
