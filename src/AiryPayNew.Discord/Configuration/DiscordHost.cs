using AiryPayNew.Discord.Utils;
using Discord;
using Discord.Addons.Hosting;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AiryPayNew.Discord.Configuration;

public static class DiscordHost
{
    public static IServiceCollection AddDiscordHost(
        this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        serviceCollection.AddDiscordHost((config, _) =>
        {
            config.SocketConfig = new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Verbose,
                AlwaysDownloadUsers = true,
                MessageCacheSize = 200,
                GatewayIntents = GatewayIntents.None
            };

            config.Token = DiscordTokenReader.GetToken();;
        });

        serviceCollection.AddCommandService((config, _) =>
        {
            config.DefaultRunMode = RunMode.Async;
            config.CaseSensitiveCommands = false;
        });

        serviceCollection.AddInteractionService((config, _) =>
        {
            config.LogLevel = LogSeverity.Info;
            config.UseCompiledLambda = true;
        });

        serviceCollection.AddInteractionHandler();
        
        return serviceCollection;
    }
}