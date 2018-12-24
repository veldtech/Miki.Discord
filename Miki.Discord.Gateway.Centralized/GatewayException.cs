using System;
using System.Collections.Generic;
using System.Text;

namespace Miki.Discord.Gateway.Centralized
{
    public class GatewayException : Exception
    {
        public GatewayException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
