using AiryPayNew.Domain.Common;
using AiryPayNew.Domain.Common.Result;
using AiryPayNew.Domain.Entities.Shops;
using AiryPayNew.Shared.Settings;
using MediatR;

namespace AiryPayNew.Application.Requests.Shops;

using Error = UpdateShopLanguageRequest.Error;

public record UpdateShopLanguageRequest(ShopId ShopId, Language Language)
    : IRequest<Result<UpdateShopLanguageRequest.Error>>
{
    public enum Error
    {
        ShopNotFound,
        LanguageNotSupported,
    }
}

public class UpdateShopLanguageRequestHandler(
    IShopRepository shopRepository,
    AppSettings appSettings)
    : IRequestHandler<UpdateShopLanguageRequest, Result<Error>>
{
    public async Task<Result<Error>> Handle(UpdateShopLanguageRequest request, CancellationToken cancellationToken)
    {
        var resultBuilder = new ResultBuilder<Error>();
        
        var shop = await shopRepository.GetByIdAsync(request.ShopId, cancellationToken);
        if (shop is null)
            return resultBuilder.WithError(Error.ShopNotFound);
        
        bool newLanguageIsSupported = appSettings.BotSupportedLanguages.Contains(request.Language);
        if (!newLanguageIsSupported)
            return resultBuilder.WithError(Error.LanguageNotSupported);
        
        await shopRepository.UpdateLanguageAsync(request.ShopId, request.Language, cancellationToken);

        return resultBuilder.WithSuccess();
    }
}