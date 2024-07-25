using AiryPayNew.Domain.Entities.Purchases;
using AiryPayNew.Domain.Entities.Shops;
using MediatR;

namespace AiryPayNew.Application.Requests.Shops;

public record GetShopPurchasesRequest(ulong ShopId) : IRequest<IList<Purchase>>;

public class GetShopPurchasesRequestHandler(
    IShopRepository shopRepository) : IRequestHandler<GetShopPurchasesRequest, IList<Purchase>>
{
    public async Task<IList<Purchase>> Handle(
        GetShopPurchasesRequest request, CancellationToken cancellationToken)
    {
        var shopId = new ShopId(request.ShopId);
        return await shopRepository.GetShopPurchasesAsync(shopId, 20, cancellationToken);
    }
}