using AiryPay.Domain.Common;
using AiryPay.Domain.Common.Result;
using AiryPay.Domain.Entities.Shops;
using AiryPay.Shared.Settings;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AiryPay.Application.Requests.Shops;

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
    AppSettings appSettings,
    ILogger<UpdateShopLanguageRequestHandler> logger)
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
        {
            logger.LogWarning("Language '{Language}' is not supported. Shop {ShopId} language not changed.",
                request.Language.Code, request.ShopId.Value);
            return resultBuilder.WithError(Error.LanguageNotSupported);
        }

        await shopRepository.UpdateLanguageAsync(request.ShopId, request.Language, cancellationToken);

        logger.LogInformation("Updated shop {ShopId} language to '{Language}'.",
            request.ShopId.Value, request.Language.Code);
        return resultBuilder.WithSuccess();
    }
}