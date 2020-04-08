namespace Miki.Discord.Common
{
    /// <summary>
    /// Token type used to differentiate bot users from normal users.
    /// </summary>
    public enum TokenType
    {
        /// <summary>
        /// API bot user.
        /// </summary>
        BOT,

        /// <summary>
        /// Discord user account.
        /// </summary>
        BEARER
    }
}
