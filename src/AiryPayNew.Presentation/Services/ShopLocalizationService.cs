using AiryPayNew.Application.Common;
using AiryPayNew.Application.Requests.Shops;
using AiryPayNew.Domain.Common;
using AiryPayNew.Presentation.Utils;
using MediatR;

namespace AiryPayNew.Presentation.Services;

public class ShopLanguageService(IMediator mediator) : IShopLanguageService
{
    public async Task<OperationResult> UpdateLanguage(ulong shopId, Language language)
    {
        var shop = await mediator.Send(new GetShopRequest(shopId));
        if (shop.Entity is null || !shop.Successful)
            return OperationResult.Error("Failed to get shop");

        return await mediator.Send(new UpdateShopLanguageRequest(shopId, language));
    }

    public async Task<OperationResult<string>> GetLocalization(ulong shopId, string localizationKey)
    {
        var shop = await mediator.Send(new GetShopRequest(shopId));
        if (shop.Entity is null || !shop.Successful)
            return OperationResult<string>.Error(string.Empty, "Failed to get shop");

        var result = LocalizationManager.GetLocalizedString(localizationKey, shop.Entity.Language);
        
        return OperationResult<string>.Success(result);
    }
}