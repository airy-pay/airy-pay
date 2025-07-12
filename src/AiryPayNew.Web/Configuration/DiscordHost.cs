using AiryPayNew.Shared.Settings.AppSettings;
using AiryPayNew.Shared.Utils;
using AiryPayNew.Web.Services;
using Discord;
using Discord.Addons.Hosting;
using Discord.WebSocket;

namespace AiryPayNew.Web.Configuration;

public static class DiscordHost
{
    public static IServiceCollection AddDiscordHost(
        this IServiceCollection serviceCollection,
        AppSettings appSettings)
    {
        serviceCollection.AddDiscordHost((config, _) =>
        {
            config.SocketConfig = new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Info,
                GatewayIntents = GatewayIntents.AllUnprivileged
            };

            config.Token = DiscordTokenReader.GetToken();;
        });

        serviceCollection.AddSingleton<InMemoryUserCache>();
        
        return serviceCollection;
    }
}