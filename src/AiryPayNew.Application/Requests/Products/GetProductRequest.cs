using AiryPayNew.Domain.Common;
using AiryPayNew.Domain.Entities.Products;
using AiryPayNew.Domain.Entities.Shops;
using MediatR;

namespace AiryPayNew.Application.Requests.Products;

public record GetProductRequest(ulong ShopId, long ProductId) : IRequest<OperationResult<Product?>>;

public class GetProductRequestHandler(
    IProductRepository productRepository) : IRequestHandler<GetProductRequest, OperationResult<Product?>>
{
    public async Task<OperationResult<Product?>> Handle(
        GetProductRequest request, CancellationToken cancellationToken)
    {
        var productId = new ProductId(request.ProductId);
        var shopId = new ShopId(request.ShopId);
        var product = await productRepository.GetByIdAsync(productId);
 
        if (product is null)
            return OperationResult<Product?>.Error(null, "Товар не найден.");
        if (product.ShopId != shopId)
            return OperationResult<Product?>.Error(product, "Нет доступа.");

        return OperationResult<Product?>.Success(product);
    }
}