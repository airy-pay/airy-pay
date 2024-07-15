using AiryPayNew.Domain.Entities.Shops;
using MediatR;

namespace AiryPayNew.Application.Requests.Shops;

public record CreateShopRequest(ulong ServerId) : IRequest;

public class CreateShopRequestHandler(IShopRepository shopRepository) : IRequestHandler<CreateShopRequest>
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
            Blocked = false
        };

        await shopRepository.Create(shop);
    }
}