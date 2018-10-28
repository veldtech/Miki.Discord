namespace Miki.Discord.Internal
{
	internal class DiscordEntity
	{
		protected DiscordClient _client;

		public DiscordEntity(DiscordClient client)
		{
			_client = client;
		}
	}
}