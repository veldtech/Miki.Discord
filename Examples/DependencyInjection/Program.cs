using Example.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Miki.Cache;
using Miki.Cache.InMemory;
using Miki.Discord.Common;
using Miki.Discord.Extensions.DependencyInjection;
using Miki.Logging;
using Miki.Serialization;
using Miki.Serialization.Protobuf;
using System;
using System.Threading.Tasks;

namespace DependencyInjection
{
    class Program
    {
        static async Task Main()
        {
            ExampleHelper.InitLog(LogLevel.Debug);

            var token = ExampleHelper.GetTokenFromEnv();

            ServiceCollection collection = new ServiceCollection();
            collection.AddSingleton<ISerializer, ProtobufSerializer>();
            collection.AddSingleton<ICacheClient, InMemoryCacheClient>();
            collection.AddSingleton<IExtendedCacheClient, InMemoryCacheClient>();
            
            collection.UseDiscord(x =>
            {
                x.Token = token;
            });

            var serviceProvider = collection.BuildServiceProvider();

            var client = serviceProvider.GetService<IDiscordClient>();

            client.Events.MessageCreate.Subscribe(x =>
            {
                Console.WriteLine($"{x.Author.Username}: {x.Content}");
            });

            await client.StartAsync(default);

            await Task.Delay(-1);
        }
    }
}
