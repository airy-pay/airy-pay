using AiryPayNew.Domain.Entities.Shops;
using AiryPayNew.Domain.Entities.Withdrawals;
using MediatR;

namespace AiryPayNew.Application.Requests.Shops;

public record GetShopWithdrawalsRequest(ShopId ShopId) : IRequest<IList<Withdrawal>>;

public class GetShopWithdrawalsRequestHandler(
    IShopRepository shopRepository) : IRequestHandler<GetShopWithdrawalsRequest, IList<Withdrawal>>
{
    public async Task<IList<Withdrawal>> Handle(
        GetShopWithdrawalsRequest request, CancellationToken cancellationToken)
    {
        return await shopRepository.GetShopWithdrawalsAsync(request.ShopId, 10, cancellationToken);
    }
}