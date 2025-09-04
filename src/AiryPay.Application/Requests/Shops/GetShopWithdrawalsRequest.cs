using AiryPay.Domain.Entities.Shops;
using AiryPay.Domain.Entities.Withdrawals;
using MediatR;

namespace AiryPay.Application.Requests.Shops;

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