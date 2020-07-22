using System;

namespace Miki.Discord.Gateway
{
    public class GatewayException : Exception
    {
        public GatewayException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
