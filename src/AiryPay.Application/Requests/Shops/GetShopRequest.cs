using AiryPay.Domain.Common.Result;
using AiryPay.Domain.Entities.Shops;
using MediatR;

namespace AiryPay.Application.Requests.Shops;

using Error = GetShopRequest.Error;

public record GetShopRequest(ShopId ShopId)
    : IRequest<Result<Shop, GetShopRequest.Error>>
{
    public enum Error
    {
        ShopNotFound
    }
}

public class GetShopRequestHandler(IShopRepository shopRepository)
    : IRequestHandler<GetShopRequest, Result<Shop, Error>>
{
    public async Task<Result<Shop, Error>> Handle(GetShopRequest request, CancellationToken cancellationToken)
    {
        var shop = await shopRepository.GetByIdNoTrackingAsync(request.ShopId, cancellationToken);
        if (shop is null)
            return Result<Shop, Error>.Fail(null!, Error.ShopNotFound);
        
        return Result<Shop, Error>.Success(shop);
    }
}