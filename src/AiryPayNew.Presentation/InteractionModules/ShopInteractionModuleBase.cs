using AiryPayNew.Application.Requests.Shops;
using AiryPayNew.Domain.Entities.Shops;
using Discord.Interactions;
using MediatR;

namespace AiryPayNew.Presentation.InteractionModules;


public abstract class ShopInteractionModuleBase(
    IMediator mediator) : InteractionModuleBase<SocketInteractionContext>
{
    private Shop? _shop;

    public Task<Shop> Shop => GetShopOrRespondAsync();

    /// <summary>
    /// The shop associated with the current Discord guild. Throws if not available.
    /// </summary>
    protected async Task<Shop> GetShopOrRespondAsync()
    {
        if (_shop is not null)
            return _shop;

        var result = await mediator.Send(new GetShopRequest(Context.Guild.Id));
        if (!result.Successful)
        {
            await RespondAsync(":no_entry_sign: " + result.ErrorMessage, ephemeral: true);
            
            throw new InvalidOperationException("Shop retrieval failed or already responded.");
        }

        _shop = result.Entity;
        
        return _shop;
    }
}