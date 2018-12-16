using System;
using System.Collections.Generic;
using System.Text;

namespace Miki.Discord.Rest.Exceptions
{
    public class DiscordRestException : Exception
    {
        readonly DiscordRestError _error;

        public DiscordRestException(DiscordRestError error)
        {
            _error = error;
        }

        public override string ToString()
        {
            return $"{_error.Code}: {_error.Message}";
        }
    }
}
