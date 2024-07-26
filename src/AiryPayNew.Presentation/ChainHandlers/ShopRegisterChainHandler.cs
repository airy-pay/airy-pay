using AiryPayNew.Application.Requests.Shops;
using Discord.Addons.ChainHandlers.ChainHandlers;
using Discord.Interactions;
using Discord.WebSocket;
using MediatR;
using IResult = Discord.Interactions.IResult;

namespace AiryPayNew.Presentation.ChainHandlers;

public class ShopRegisterChainHandler(
    IServiceProvider provider,
    InteractionService interactionService,
    DiscordSocketClient socketClient,
    IServiceScopeFactory serviceScopeFactory)
    : ChainHandler(provider, interactionService, socketClient)
{
    public override async Task<IResult> Handle(SocketInteraction interaction)
    {
        if (interaction.GuildId is null)
            throw new InvalidDataException("Interaction guild id was null");

        using var scope = serviceScopeFactory.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        var createShop = new CreateShopRequest(interaction.GuildId.Value);
        await mediator.Send(createShop);
        
        return await base.Handle(interaction);
    }
}