using System.Text.Json;

namespace Miki.Discord.Gateway
{
    public static class JsonElementExtensions
    {
        public static T ToObject<T>(this JsonElement element, JsonSerializerOptions options = null)
        {
            return JsonSerializer.Deserialize<T>(element.GetRawText(), options);
        }   
    }
}
