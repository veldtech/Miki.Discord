using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;
using Miki.Discord.Common.Gateway;
using Miki.Discord.Gateway.Connection;
using Miki.Serialization;
using Miki.Serialization.Protobuf;
using Xunit;

namespace Miki.Discord.Tests.Gateway
{
    public class PacketTest
    {
        [Theory]
        [InlineData(GatewayOpcode.Dispatch, "GATEWAY_EVENT_NAME", @"{""op"":0,""d"":{},""s"":42,""t"":""GATEWAY_EVENT_NAME""}")]
        [InlineData(GatewayOpcode.InvalidSession, null, @"{""op"":9,""d"":true}")]
        public void TestParsePacket(GatewayOpcode? opCode, string eventName, string json)
        {
            var packet = GatewayMessageIdentifier.Read(Encoding.UTF8.GetBytes(json));

            Assert.Equal(opCode, packet.OpCode);
            Assert.Equal(eventName, packet.EventName);
        }
    }
}
