using AiryPayNew.Discord.ChainHandlers;
using AiryPayNew.Shared.Settings;
using Discord;
using Discord.Addons.ChainHandlers;
using Discord.Addons.ChainHandlers.Default;
using Discord.Interactions;
using Microsoft.Extensions.DependencyInjection;

namespace AiryPayNew.Discord.Configuration;

public static class Interaction
{
    public static IServiceCollection AddInteractionHandler(
        this IServiceCollection serviceCollection,
        AppSettings appSettings)
    {
        serviceCollection.AddInteractionHandler(options =>
        {
            options.UseChainHandler(handlerOptions =>
            {
                handlerOptions
                    .Add<ErrorChainHandler>()
                    .Add<ProblemChainHandler>()
                    .Add<RateLimitChainHandler>()
                    .Add<ShopRegisterChainHandler>();
            })
                .UseFinalHandler(ConfigureFinalHandler)
                .ConfigureInteractionService(ConfigureCommands, appSettings);
        });

        return serviceCollection;
    }

    private static async void ConfigureFinalHandler(IInteractionContext interactionContext)
    {
        await interactionContext.Interaction.RespondAsync(":x: Что-то пошло не так", ephemeral: true);
    }

    private static async void ConfigureCommands(InteractionService interactionService, AppSettings appSettings)
    {
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