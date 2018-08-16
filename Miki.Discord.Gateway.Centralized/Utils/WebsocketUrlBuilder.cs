using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Miki.Discord.Gateway.Centralized.Utils
{
	public static class DictionaryUtils
	{
		public static void AddOrUpdate<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, TValue value)
		{
			if (dict.ContainsKey(key))
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

    public class WebsocketUrlBuilder
    {
		string url = GatewayConstants.BaseUrl;
		Dictionary<string, object> arguments = new Dictionary<string, object>();

		public WebsocketUrlBuilder SetVersion(int version = GatewayConstants.DefaultVersion)
		{
			arguments.AddOrUpdate("v", version);
			return this;
		}

		public WebsocketUrlBuilder SetEncoding(GatewayEncoding encoding)
		{
			arguments.AddOrUpdate("encoding", encoding);
			return this;
		}

		public WebsocketUrlBuilder SetCompression(bool compressed)
		{
			if (compressed)
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

		public static string FromGatewayConfiguration(GatewayConfiguration gatewayConfiguration)
		{
			WebsocketUrlBuilder builder = new WebsocketUrlBuilder();
			builder.SetVersion(gatewayConfiguration.Version);
			builder.SetCompression(gatewayConfiguration.Compressed);
			builder.SetEncoding(gatewayConfiguration.Encoding);
			return builder.Build();
		}
	}
}
