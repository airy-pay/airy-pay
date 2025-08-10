using AiryPayNew.Domain.Common.Result;
using AiryPayNew.Domain.Entities.Products;
using AiryPayNew.Domain.Entities.Shops;
using MediatR;

namespace AiryPayNew.Application.Requests.Products;

using Error = GetProductRequest.Error;

public record GetProductRequest(ShopId ShopId, long ProductId)
    : IRequest<Result<Product, GetProductRequest.Error>>
{
    public enum Error
    {
        ProductNotFound,
        Unauthorized
    }
}

public class GetProductRequestHandler(
    IProductRepository productRepository)
    : IRequestHandler<GetProductRequest, Result<Product, Error>>
{
    public async Task<Result<Product, Error>> Handle(
        GetProductRequest request, CancellationToken cancellationToken)
    {
        var resultBuilder = new ResultBuilder<Product, Error>(null!);
        
        var productId = new ProductId(request.ProductId);
        var product = await productRepository.GetByIdNoTrackingAsync(productId, cancellationToken);
        
        if (product is null)
            return resultBuilder.WithError(Error.ProductNotFound);
        if (product.ShopId != request.ShopId)
            return resultBuilder.WithError(Error.Unauthorized);

        return resultBuilder.WithSuccess(product);
    }
}