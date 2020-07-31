using Microsoft.Extensions.DependencyInjection;
using Miki.Cache;
using Miki.Discord.Common;
using Miki.Discord.Gateway;
using Miki.Discord.Rest;
using System;

namespace Miki.Discord.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection UseDiscord(
            this IServiceCollection collection, Action<DiscordConfiguration> configFactory)
            => collection.UseDiscord((provider, config) => configFactory(config));

        public static IServiceCollection UseDiscord(
            this IServiceCollection collection,
            Action<IServiceProvider, DiscordConfiguration> configFactory)
        {
            collection.AddSingleton(x =>
            {
                var config = new DiscordConfiguration();
                configFactory(x, config);
                if (!config.Token.IsValidToken())
                {
                    throw new ArgumentException("Invalid Token");
                }

                if (config.GatewayProperties.Token == null)
                {
                    config.GatewayProperties.Token = config.Token.ToString();
                }

                return config;
            });

            collection.AddSingleton<IApiClient>(x =>
            {
                return new DiscordApiClient(
                    x.GetRequiredService<DiscordConfiguration>().Token,
                    x.GetRequiredService<ICacheClient>());
            });
            collection.AddSingleton<IGateway>(x => 
                new GatewayCluster(x.GetRequiredService<DiscordConfiguration>().GatewayProperties));
            collection.AddSingleton<IDiscordClient, DiscordClient>();
            return collection;
        }
    }
}