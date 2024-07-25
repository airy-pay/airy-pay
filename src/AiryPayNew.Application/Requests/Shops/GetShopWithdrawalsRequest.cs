using AiryPayNew.Domain.Entities.Shops;
using AiryPayNew.Domain.Entities.Withdrawals;
using MediatR;

namespace AiryPayNew.Application.Requests.Shops;

public record GetShopWithdrawalsRequest(ulong ShopId) : IRequest<IList<Withdrawal>>;

public class GetShopWithdrawalsRequestHandler(
    IShopRepository shopRepository) : IRequestHandler<GetShopWithdrawalsRequest, IList<Withdrawal>>
{
    public async Task<IList<Withdrawal>> Handle(
        GetShopWithdrawalsRequest request, CancellationToken cancellationToken)
    {
        var shopId = new ShopId(request.ShopId);
        return await shopRepository.GetShopWithdrawalsAsync(shopId, 10, cancellationToken);
    }
}