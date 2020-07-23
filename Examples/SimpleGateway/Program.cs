using Example.Helpers;
using Miki.Cache.InMemory;
using Miki.Discord;
using Miki.Discord.Common;
using Miki.Discord.Gateway;
using Miki.Discord.Rest;
using Miki.Logging;
using Miki.Serialization.Protobuf;
using System;
using System.Threading.Tasks;

namespace SimpleGateway
{
    // This is a test app for Miki.Discord's current interfacing. The goal of this app is to explain how
    // the system works and why these steps are necessary. If you're new to bot programming, this might
    // not be the most suitable API for you, but it does promiseScaling and control over certain aspects
    // whenever needed. For questions join the Miki Stack discord (link in README.md) or tweet me
    // @velddev
    internal static class Program
	{
        static async Task Main()
		{
            // Sets up Miki.Logging for internal library logging. Can be removed if you do not want to
            // see internal logs.
            ExampleHelper.InitLog(LogLevel.Debug);

            // Fetches your token from environment values.
            var token = ExampleHelper.GetTokenFromEnv();

            var memCache = new InMemoryCacheClient(new ProtobufSerializer());
            
            var apiClient = new DiscordApiClient(token, memCache);
            
            // Discord direct gateway implementation. 
            var gateway = new GatewayCluster(
	            new GatewayProperties
				{
					ShardCount = 1,
					ShardId = 0,
	                Token = token.ToString(),
	            });
            
            var discordClient = new DiscordClient(apiClient, gateway, memCache);

            // Subscribe to ready event.
            discordClient.Events.MessageCreate.SubscribeTask(OnMessageReceived);

            // Start the connection to the gateway.
            await gateway.StartAsync();

			// Wait, else the application will close.
			await Task.Delay(-1);
		}

        static async Task OnMessageReceived(IDiscordMessage message)
        {
            if (message.Content == "ping")
            {
                var channel = await message.GetChannelAsync();
                await channel.SendMessageAsync("pong!");
            }
        }
    }
}