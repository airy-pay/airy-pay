using AiryPayNew.Application.Common;
using AiryPayNew.Application.Requests.Shops;
using AiryPayNew.Domain.Common;
using AiryPayNew.Discord.Utils;
using MediatR;

namespace AiryPayNew.Discord.Services;

public class ShopLanguageService(IMediator mediator) : IShopLanguageService
{
    public async Task<OperationResult> UpdateLanguage(ulong shopId, Language language)
    {
        var shop = await mediator.Send(new GetShopRequest(shopId));
        if (shop.Entity is null || !shop.Successful)
            return OperationResult.Error("Failed to get shop");

        return await mediator.Send(new UpdateShopLanguageRequest(shopId, language));
    }
}