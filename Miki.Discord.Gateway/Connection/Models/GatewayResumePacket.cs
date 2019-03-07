using Newtonsoft.Json;

namespace Miki.Discord.Gateway.Connection
{
    public class GatewayResumePacket
    {
        [JsonProperty("token")]
        public string Token { get; set; }

        [JsonProperty("session_id")]
        public string SessionId { get; set; }

        [JsonProperty("seq")]
        public int Sequence { get; set; }
    }
}