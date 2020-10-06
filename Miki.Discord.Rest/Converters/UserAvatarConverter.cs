using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Miki.Discord.Common.Packets;

namespace Miki.Discord.Rest.Converters
{
    public class UserAvatarConverter : JsonConverter<UserAvatar>
    {
        public override UserAvatar Read(
            ref Utf8JsonReader reader, Type objectType, JsonSerializerOptions serializer)
        {
            // Never need to be read.
            throw new NotSupportedException();
        }

        public override void Write(
            Utf8JsonWriter writer, UserAvatar value, JsonSerializerOptions serializer)
        {
            if(value == null)
            {
                writer.WriteNullValue();
                return;
            }

            string imageData = Convert.ToBase64String(value.Stream.GetBuffer());
            writer.WriteStringValue($"data:image/{value.Type.ToString().ToLower()};base64,{imageData}");
        }
    }
}
