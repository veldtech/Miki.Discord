using System;
using System.Collections.Generic;
using System.Text;

namespace Miki.Discord.Common.Utils
{
    public struct DiscordToken
    {
        public DiscordToken(string tokenSource)
        {
            if (tokenSource
                .ToLowerInvariant()
                .StartsWith("bearer "))
            {
                Token = tokenSource.Substring(7);
                Type = TokenType.BEARER;
            }
            else if (tokenSource
                .ToLowerInvariant()
                .StartsWith("bot "))
            {
                Token = tokenSource.Substring(4);
                Type = TokenType.BOT;
            }
            else
            {
                Token = tokenSource;
                Type = TokenType.BOT;
            }
        }

        public string Token { get; }
        public TokenType Type { get; }

        public string GetOAuthType()
        {
            var x = Type.ToString()
                .ToLowerInvariant()
                .ToCharArray();
            x[0] = char.ToUpperInvariant(x[0]);
            return new string(x);
        }

        public static implicit operator DiscordToken(string token)
            => new DiscordToken(token);
    }
}
