using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace Miki.Discord.Gateway.Connection
{
    public class GatewayResumePacket
    {
        [JsonProperty("token")]
        [JsonPropertyName("token")]
        public string Token { get; set; }

        [JsonProperty("session_id")]
        [JsonPropertyName("session_id")]
        public string SessionId { get; set; }

        [JsonProperty("seq")]
        [JsonPropertyName("seq")]
        public int Sequence { get; set; }
    }
}