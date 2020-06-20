namespace Miki.Discord.Gateway
{
    using System;

    public class GatewayException : Exception
    {
        public GatewayException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
