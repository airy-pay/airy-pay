using AiryPayNew.Domain.Common;
using AiryPayNew.Domain.Entities.Shops;
using AiryPayNew.Shared.Settings.AppSettings;
using MediatR;

namespace AiryPayNew.Application.Requests.Shops;

public record UpdateShopLanguageRequest(ulong ShopId, Language Language) : IRequest<OperationResult>;

public class UpdateShopLanguageRequestHandler(
    IShopRepository shopRepository,
    AppSettings appSettings) : IRequestHandler<UpdateShopLanguageRequest, OperationResult>
{
    public async Task<OperationResult> Handle(UpdateShopLanguageRequest request, CancellationToken cancellationToken)
    {
        var shopId = new ShopId(request.ShopId);
        
        var shop = await shopRepository.GetByIdAsync(shopId, cancellationToken);
        if (shop is null)
            return new OperationResult(false, "Shop not found");
        
        bool newLanguageIsSupported = appSettings.BotSupportedLanguages.Contains(request.Language);
        if (!newLanguageIsSupported)
            return new OperationResult(false, "Language is not supported");
        
        return await shopRepository.UpdateLanguageAsync(shopId, request.Language, cancellationToken);
    }
}