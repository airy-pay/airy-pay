using AiryPayNew.Domain.Entities.Products;
using AiryPayNew.Domain.Entities.Shops;
using MediatR;

namespace AiryPayNew.Application.Requests.Products;

public record RemoveProductRequest(ulong ServerId, ProductId ProductId) : IRequest;

public class RemoveProductRequestHandler(IProductRepository productRepository) : IRequestHandler<RemoveProductRequest>
{
    public async Task Handle(RemoveProductRequest request, CancellationToken cancellationToken)
    {
        var product = await productRepository.GetByIdAsync(request.ProductId);
        if (product is null)
            return;
        
        var shopId = new ShopId(request.ServerId);
        if (product.ShopId != shopId)
            return;
        
        await productRepository.Delete(request.ProductId);
    }
}