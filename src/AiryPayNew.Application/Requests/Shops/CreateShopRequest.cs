using AiryPayNew.Domain.Entities.Shops;
using AiryPayNew.Shared.Settings.AppSettings;
using MediatR;

namespace AiryPayNew.Application.Requests.Shops;

public record CreateShopRequest(ulong ServerId) : IRequest;

public class CreateShopRequestHandler(
    IShopRepository shopRepository,
    AppSettings appSettings) : IRequestHandler<CreateShopRequest>
{
    public async Task Handle(CreateShopRequest request, CancellationToken cancellationToken)
    {
        var shopId = new ShopId(request.ServerId);
        var shop = await shopRepository.GetByIdNoTrackingAsync(shopId);
        if (shop is not null)
            return;

        shop = new Shop
        {
            Id = shopId,
            Balance = 0,
            Blocked = false,
            Commission = new Commission(appSettings.PaymentSettings.DefaultShopCommission)
        };

        await shopRepository.Create(shop);
    }
}