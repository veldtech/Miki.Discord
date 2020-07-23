using System;

namespace Miki.Discord.Common
{
    /// <summary>
    /// Discord Token wrapper object to abstractify the bare token away.
    /// </summary>
    public struct DiscordToken
    {
        /// <summary>
        /// Parses token for multiple kinds of tokens.
        /// <list type="number">
        /// <item>
        /// <term>Bearer {{TOKEN}}</term>
        /// <description>For a user login token.</description>
        /// </item>
        /// <item>
        /// <term>Bot {{TOKEN}}</term>
        /// <description>For a bot API token.</description>
        /// </item>
        /// <item>
        /// <term>{{TOKEN}}</term>
        /// <description>Defaults to Bot.</description>
        /// </item>
        /// </list>
        /// </summary>
        public DiscordToken(string tokenSource)
        {
            if(tokenSource.ToLowerInvariant().StartsWith("bearer "))
            {
                Token = tokenSource.Substring(7);
                Type = TokenType.BEARER;
            }
            else if(tokenSource.ToLowerInvariant().StartsWith("bot "))
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

        /// <summary>
        /// Raw token source
        /// </summary>
        public string Token { get; }

        /// <summary>
        /// Token type
        /// </summary>
        public TokenType Type { get; }

        /// <summary>
        /// Gets the formatted type. e.g. Bot, Bearer.
        /// </summary>
        public string GetOAuthType()
        {
            var x = Type.ToString()
                .ToLowerInvariant()
                .ToCharArray();
            x[0] = char.ToUpperInvariant(x[0]);
            return new string(x);
        }

        /// <summary>
        /// Verifies if the token is somewhat
        /// </summary>
        /// <returns></returns>
        public bool IsValidToken()
        {
            if (Token == null)
            {
                return false;
            }

            var segments = Token.Split('.');
            if (segments.Length != 3)
            {
                return false;
            }

            return true;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return GetOAuthType() + " " + Token;
        }

        /// <summary>
        /// Implicitely takes a string and parses it into a token.
        /// </summary>
        public static implicit operator DiscordToken(string token)
            => new DiscordToken(token);
    }
}
