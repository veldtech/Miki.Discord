using Miki.Discord.Common.Packets.Arguments;
using Newtonsoft.Json;
using System;

namespace Miki.Discord.Rest.Converters
{
	public class UserAvatarConverter : JsonConverter<UserAvatar>
	{
		public override UserAvatar ReadJson(JsonReader reader, Type objectType, UserAvatar existingValue, bool hasExistingValue, JsonSerializer serializer)
		{
			// Never need to be read.
			throw new NotSupportedException();
		}

		public override void WriteJson(JsonWriter writer, UserAvatar value, JsonSerializer serializer)
		{
			if(value == null)
			{
				writer.WriteNull();
				return;
			}
			string imageData = Convert.ToBase64String(value.Stream.GetBuffer());
			writer.WriteValue($"data:image/{value.Type.ToString().ToLower()};base64,{imageData}");
		}
	}
}
