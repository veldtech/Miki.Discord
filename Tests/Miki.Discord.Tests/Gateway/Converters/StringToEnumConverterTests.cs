using Miki.Discord.Gateway.Converters;
using System.IO;
using System.Text.Json;
using Xunit;

namespace Miki.Discord.Tests.Gateway.Converters
{
    internal enum TestEnum : long
    {
        A = 1,
        B = 2
    }

    internal struct TestObject
    {
        public TestEnum Enum { get; set; }
    }

    public class StringToEnumConverterTests
    {
        private JsonSerializerOptions options;

        public StringToEnumConverterTests()
        {
            options = new JsonSerializerOptions
            {
                Converters = { new StringToEnumConverter<TestEnum>() }
            };
        }

        [Fact]
        public void ValueToJsonString()
        {
            var value = new TestObject
            {
                Enum = TestEnum.A
            };

            var jsonString = JsonSerializer.Serialize(value, options);
            Assert.Equal("{\"Enum\":\"1\"}", jsonString);
        }

        [Fact]
        public void JsonStringToValue()
        {
            var json = "{\"Enum\":\"1\"}";
            var value = JsonSerializer.Deserialize<TestObject>(json, options);
            Assert.Equal(TestEnum.A, value.Enum);
        }
    }
}
