using Miki.Discord.Gateway.Connection;
using System.Collections.Generic;
using System.Linq;

namespace Miki.Discord.Gateway.Utils
{
	public static class DictionaryUtils
	{
		public static void AddOrUpdate<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, TValue value)
		{
			if(dict.ContainsKey(key))
			{
				dict[key] = value;
			}
			else
			{
				dict.Add(key, value);
			}
		}

		public static bool TryRemove<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key)
		{
			if(dict.ContainsKey(key))
			{
				dict.Remove(key);
				return true;
			}
			return false;
		}
	}

	public class WebSocketUrlBuilder
	{
		private readonly string url;
		private readonly Dictionary<string, object> arguments = new Dictionary<string, object>();

		public WebSocketUrlBuilder(string baseUrl)
		{
			url = baseUrl;
		}

		public WebSocketUrlBuilder SetVersion(int version = GatewayConstants.DefaultVersion)
		{
			arguments.AddOrUpdate("v", version);
			return this;
		}

		public WebSocketUrlBuilder SetEncoding(GatewayEncoding encoding)
		{
			arguments.AddOrUpdate("encoding", encoding.ToString().ToLower());
			return this;
		}

		public WebSocketUrlBuilder SetCompression(bool compressed)
		{
			if(compressed)
			{
				arguments.AddOrUpdate("compress", "zlib-stream");
			}
			else
			{
				arguments.TryRemove("compress");
			}
			return this;
		}

		public string Build()
		{
			if(arguments.Count == 0)
			{
				return url;
			}
			return url + "?" + string.Join("&", arguments.Select(x => $"{x.Key}={x.Value}"));
		}
	}
}