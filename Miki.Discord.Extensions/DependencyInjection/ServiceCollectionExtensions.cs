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
        {
            var config = new DiscordConfiguration();
            configFactory(config);

            if (!config.Token.IsValidToken())
            {
                throw new ArgumentException("Invalid Token");
            }

            if(config.GatewayProperties.Token == null)
            {
                config.GatewayProperties.Token = config.Token.ToString();
            }

            collection.AddSingleton(config);
            collection.UseDiscord();

            return collection;
        }
        public static IServiceCollection UseDiscord(
            this IServiceCollection collection)
        {
            collection.AddSingleton<IApiClient, DiscordApiClient>(
                x => new DiscordApiClient(
                    x.GetRequiredService<DiscordConfiguration>().Token, 
                    x.GetRequiredService<ICacheClient>()));
            collection.AddSingleton<IGateway, GatewayCluster>(
                x => new GatewayCluster(
                    x.GetRequiredService<DiscordConfiguration>().GatewayProperties));
            collection.AddSingleton<IDiscordClient, DiscordClient>();
            return collection;
        }
    }
}