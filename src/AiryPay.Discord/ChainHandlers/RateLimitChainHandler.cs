using Discord.Addons.ChainHandlers.ChainHandlers;
using Discord.Addons.ChainHandlers.Common;
using Discord.Interactions;
using Discord.WebSocket;
using GenericRateLimiter;
using GenericRateLimiter.Core;
using IResult = Discord.Interactions.IResult;

namespace AiryPay.Discord.ChainHandlers;

public class RateLimitChainHandler(
    IServiceProvider provider,
    InteractionService interactionService,
    DiscordSocketClient socketClient,
    IEntityRateLimiter<ulong> rateLimiter)
    : ChainHandler(provider, interactionService, socketClient)
{
    public override async Task<IResult> Handle(SocketInteraction interaction)
    {
        var rateLimitStatus =  rateLimiter.Trigger(interaction.User.Id);

        if (rateLimitStatus == RateLimitStatus.Accessible)
            return await base.Handle(interaction);
        
        await interaction.RespondAsync(":alarm_clock: You send messages too fast", ephemeral: true);
        return InteractionResult.Success;
    }
}