using System;

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
			return $"{nameof(DiscordRestException)}: {_error.Code} - {_error.Message}\n{StackTrace}";
		}
	}
}
