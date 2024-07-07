using AiryPayNew.Domain.Common;
using AiryPayNew.Domain.Entities.Products;
using AiryPayNew.Domain.Entities.Shops;
using MediatR;

namespace AiryPayNew.Application.Requests.Products;

public record GetProductsFromShopRequest(ulong ServerId) : IRequest<OperationResult<IList<Product>>>;

public class GetProductsFromShopRequestHandler(IShopRepository shopRepository)
    : IRequestHandler<GetProductsFromShopRequest, OperationResult<IList<Product>>>
{
    public async Task<OperationResult<IList<Product>>> Handle(GetProductsFromShopRequest request, CancellationToken cancellationToken)
    {
        var shopId = new ShopId(request.ServerId);
        var shop = await shopRepository.GetByIdAsync(shopId);
        if (shop is null)
            return OperationResult<IList<Product>>.Error([], "Магазин не найден");

        return OperationResult<IList<Product>>.Success(shop.Products);
    }
}