using AiryPayNew.Domain.Common;
using AiryPayNew.Domain.Entities.Shops;
using MediatR;

namespace AiryPayNew.Application.Requests.Shops;

public record GetShopRequest(ulong ShopId) : IRequest<OperationResult<Shop?>>;

public class GetShopRequestHandler(IShopRepository shopRepository) : IRequestHandler<GetShopRequest, OperationResult<Shop?>>
{
    public async Task<OperationResult<Shop?>> Handle(GetShopRequest request, CancellationToken cancellationToken)
    {
        var shopId = new ShopId(request.ShopId);
        var shop = await shopRepository.GetByIdNoTrackingAsync(shopId, cancellationToken);
        return shop is null
            ? OperationResult<Shop?>.Error(null, "Магазин не найден")
            : OperationResult<Shop?>.Success(shop);
    }
}