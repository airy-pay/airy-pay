using AiryPayNew.Domain.Common.Result;
using AiryPayNew.Domain.Entities.Shops;
using MediatR;

namespace AiryPayNew.Application.Requests.Shops;

using Error = GetShopRequest.Error;

public record GetShopRequest(ulong ShopId)
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
        var shopId = new ShopId(request.ShopId);
        var shop = await shopRepository.GetByIdNoTrackingAsync(shopId, cancellationToken);
        if (shop is null)
            return Result<Shop, Error>.Fail(null!, Error.ShopNotFound);
        
        return Result<Shop, Error>.Success(shop);
    }
}