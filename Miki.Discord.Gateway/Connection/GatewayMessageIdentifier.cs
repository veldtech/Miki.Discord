using System;
using System.Text;
using Miki.Discord.Common.Gateway;

namespace Miki.Discord.Gateway.Connection
{
    /// <summary>
    ///     The identifier of the gateway message.
    ///     This will read only the OpCode and EventName.
    /// </summary>
    public struct GatewayMessageIdentifier
    {
        private const string OpCodePrefix = @"""op""";
        private const string EventNamePrefix = @"""t""";
        private static readonly ReadOnlyMemory<byte> OpCodePrefixMemory = Encoding.UTF8.GetBytes(OpCodePrefix);
        private static readonly ReadOnlyMemory<byte> EventNamePrefixMemory = Encoding.UTF8.GetBytes(EventNamePrefix);

        public GatewayOpcode? OpCode;
        public string EventName;

        public static GatewayMessageIdentifier Read(ReadOnlySpan<byte> data)
        {
            var opCode = ReadOpCode(data);

            return new GatewayMessageIdentifier
            {
                OpCode = opCode,
                EventName = opCode == GatewayOpcode.Dispatch ? ReadEventName(data) : null
            };
        }

        private static GatewayOpcode? ReadOpCode(ReadOnlySpan<byte> data)
        {
            const byte colonByte = (byte)':';
            const byte commaByte = (byte)',';

            var eventNameIndex = data.IndexOf(OpCodePrefixMemory.Span);

            if (eventNameIndex == -1)
            {
                return default;
            }

            data = data.Slice(eventNameIndex + OpCodePrefix.Length);
            data = data.Slice(data.IndexOf(colonByte) + 1);
            data = data.Slice(0, data.IndexOf(commaByte));

#if NETSTANDARD2_0
            var num = Encoding.UTF8.GetString(data.ToArray()).Trim();
#else
            var num = Encoding.UTF8.GetString(data).Trim();
#endif

            return (GatewayOpcode)int.Parse(num);
        }

        private static string ReadEventName(ReadOnlySpan<byte> data)
        {
            const byte stringByte = (byte) '"';

            var eventNameIndex = data.IndexOf(EventNamePrefixMemory.Span);

            if (eventNameIndex == -1)
            {
                return default;
            }

            data = data.Slice(eventNameIndex + EventNamePrefix.Length);
            data = data.Slice(data.IndexOf(stringByte) + 1);
            data = data.Slice(0, data.IndexOf(stringByte));

#if NETSTANDARD2_0
            return Encoding.UTF8.GetString(data.ToArray());
#else
            return Encoding.UTF8.GetString(data);
#endif
        }
    }
}
