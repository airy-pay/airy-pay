using AiryPayNew.Domain.Common.Result;
using AiryPayNew.Domain.Entities.Products;
using AiryPayNew.Domain.Entities.Shops;
using MediatR;

namespace AiryPayNew.Application.Requests.Products;

using Error = GetProductsFromShopRequest.Error;

public record GetProductsFromShopRequest(ShopId ShopId)
    : IRequest<Result<IList<Product>, GetProductsFromShopRequest.Error>>
{
    public enum Error
    {
        ShopNotFound
    }
}

public class GetProductsFromShopRequestHandler(IShopRepository shopRepository)
    : IRequestHandler<GetProductsFromShopRequest, Result<IList<Product>, Error>>
{
    public async Task<Result<IList<Product>, Error>> Handle(
        GetProductsFromShopRequest request, CancellationToken cancellationToken)
    {
        var shop = await shopRepository.GetByIdNoTrackingAsync(request.ShopId, cancellationToken);
        if (shop is null)
        {
            return Result<IList<Product>, Error>.Fail(
                new List<Product>(), Error.ShopNotFound);
        }

        return Result<IList<Product>, Error>.Success(shop.Products);
    }
}