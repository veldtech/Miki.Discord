using Miki.Discord.Common.Gateway;
using Miki.Discord.Common.Gateway.Packets;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Miki.Discord.Common.Converters
{
    public class GatewayMessageConverter : JsonConverter<GatewayMessage>
    {
        public override GatewayMessage ReadJson(JsonReader reader, Type objectType, GatewayMessage existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var val = existingValue;
            if (!hasExistingValue)
            {
                val = new GatewayMessage();
            }

            // StartObject read
            reader.Read();

            val.EventName = reader.ReadAsString();
            reader.Read();

            val.SequenceNumber = reader.ReadAsInt32();
            reader.Read();

            // Read OpCode
            val.OpCode = (GatewayOpcode)reader.ReadAsInt32();
            reader.Read();

            // Ignore data name
            reader.Read();

            val.Data = JToken.ReadFrom(reader);
            reader.Read();

            return val;
        }

        public override void WriteJson(JsonWriter writer, GatewayMessage value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
