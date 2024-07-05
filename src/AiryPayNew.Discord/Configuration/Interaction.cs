using AiryPayNew.Discord.ChainHandlers;
using AiryPayNew.Discord.Utils;
using Discord;
using Discord.Addons.ChainHandlers;
using Discord.Addons.ChainHandlers.Default;
using Discord.Interactions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AiryPayNew.Discord.Configuration;

public static class Interaction
{
    public static IServiceCollection AddInteractionHandler(
        this IServiceCollection serviceCollection,
        IConfiguration configuration)
    {
        serviceCollection.AddInteractionHandler(options =>
        {
            options.UseChainHandler(handlerOptions =>
            {
                handlerOptions
                    .Add<ErrorChainHandler>()
                    .Add<ProblemChainHandler>()
                    .Add<RateLimitChainHandler>();
            });

            options.UseFinalHandler(ConfigureFinalHandler);
            options.ConfigureInteractionService(ConfigureCommands);
        });

        return serviceCollection;
    }

    private static async void ConfigureFinalHandler(IInteractionContext interactionContext)
    {
        await interactionContext.Interaction.RespondAsync(":x: Что-то пошло не так", ephemeral: true);
    }

    private static async void ConfigureCommands(InteractionService interactionService, IConfiguration configuration)
    {
        var appSettings = configuration.GetAppSettings();

        if (appSettings.Discord.UseStagingServer)
        {
            await interactionService.AddModulesToGuildAsync(appSettings.Discord.StagingServerId, true, []);
            await interactionService.RegisterCommandsToGuildAsync(appSettings.Discord.StagingServerId);
        }
        else
        {
            await interactionService.AddModulesGloballyAsync(true, []);
            await interactionService.RegisterCommandsGloballyAsync();
        }
    }
}