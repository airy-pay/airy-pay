using AiryPay.Application.Requests.Shops;
using AiryPay.Discord.Localization;
using AiryPay.Domain.Entities.Shops;
using Discord.Interactions;
using MediatR;

namespace AiryPay.Discord.InteractionModules;

public abstract class ShopInteractionModuleBase(
    IMediator mediator) : InteractionModuleBase<SocketInteractionContext>
{
    private Shop? _shop;
    private Localizer? _localizer;

    public Task<Shop> Shop => GetShopOrRespondAsync();
    public ShopId ShopId;

    /// <summary>
    /// The shop associated with the current Discord guild. Throws if not available.
    /// </summary>
    protected async Task<Shop> GetShopOrRespondAsync()
    {
        if (_shop is not null)
            return _shop;

        var getShopRequest = new GetShopRequest(
            new ShopId(Context.Guild.Id));
        var result = await mediator.Send(getShopRequest);
        if (result.Failed)
        {
            await RespondAsync(":no_entry_sign: Shop not found", ephemeral: true);

            throw new InvalidOperationException("Shop retrieval failed or already responded.");
        }

        _shop = result.Entity;
        ShopId = _shop.Id;

        return _shop;
    }

    protected async Task<(Shop shop, Localizer localizer)> GetShopAndLocalizerAsync()
    {
        var shop = await GetShopOrRespondAsync();
        _localizer ??= new Localizer(shop.Language);
        return (shop, _localizer);
    }
}