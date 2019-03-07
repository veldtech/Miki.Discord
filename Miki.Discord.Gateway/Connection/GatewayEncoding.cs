namespace Miki.Discord.Gateway.Connection
{
    /// <summary>
    /// Discord supported Gateway encoding formats
    /// </summary>
	public enum GatewayEncoding
	{
        /// <summary>
        /// Plain-text Json
        /// </summary>
		Json,

        /// <summary>
        /// Erlang binary format
        /// </summary>
		ETF
	}
}