using AiryPay.Domain.Entities.Purchases;
using AiryPay.Domain.Entities.Shops;
using MediatR;

namespace AiryPay.Application.Requests.Shops;

public record GetShopPurchasesRequest(ShopId ShopId) : IRequest<IList<Purchase>>;

public class GetShopPurchasesRequestHandler(
    IShopRepository shopRepository) : IRequestHandler<GetShopPurchasesRequest, IList<Purchase>>
{
    public async Task<IList<Purchase>> Handle(
        GetShopPurchasesRequest request, CancellationToken cancellationToken)
    {
        return await shopRepository.GetShopPurchasesAsync(request.ShopId, 20, cancellationToken);
    }
}