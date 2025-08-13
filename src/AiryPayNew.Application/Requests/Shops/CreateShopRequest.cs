using AiryPayNew.Domain.Entities.Shops;
using AiryPayNew.Shared.Settings;
using MediatR;

namespace AiryPayNew.Application.Requests.Shops;

public record CreateShopRequest(ShopId ShopId) : IRequest;

public class CreateShopRequestHandler(
    IShopRepository shopRepository,
    AppSettings appSettings) : IRequestHandler<CreateShopRequest>
{
    public async Task Handle(CreateShopRequest request, CancellationToken cancellationToken)
    {
        var shop = await shopRepository.GetByIdNoTrackingAsync(request.ShopId, cancellationToken);
        if (shop is not null)
            return;

        shop = new Shop
        {
            Id = request.ShopId,
            Balance = 0,
            Blocked = false,
            Commission = new Commission(appSettings.PaymentSettings.DefaultShopCommission),
            Language = appSettings.BotSupportedLanguages.First()
        };

        await shopRepository.CreateAsync(shop, cancellationToken);
    }
}