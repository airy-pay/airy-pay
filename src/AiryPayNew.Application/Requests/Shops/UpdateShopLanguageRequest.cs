using AiryPayNew.Domain.Common;
using AiryPayNew.Domain.Entities.Shops;
using AiryPayNew.Shared.Settings.AppSettings;
using MediatR;

namespace AiryPayNew.Application.Requests.Shops;

public record UpdateShopLanguageRequest(ShopId ShopId, Language Language) : IRequest<OperationResult>;

public class UpdateShopLanguageRequestHandler(
    IShopRepository shopRepository,
    AppSettings appSettings) : IRequestHandler<UpdateShopLanguageRequest, OperationResult>
{
    public async Task<OperationResult> Handle(UpdateShopLanguageRequest request, CancellationToken cancellationToken)
    {
        var shop = await shopRepository.GetByIdAsync(request.ShopId, cancellationToken);
        if (shop is null)
            return new OperationResult(false, "Shop not found");
        
        bool newLanguageIsSupported = appSettings.BotSupportedLanguages.Contains(request.Language);
        if (!newLanguageIsSupported)
            return new OperationResult(false, "Language is not supported");
        
        return await shopRepository.UpdateLanguageAsync(request.ShopId, request.Language, cancellationToken);
    }
}