using AiryPayNew.Domain.Entities.Products;
using AiryPayNew.Domain.Entities.Shops;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AiryPayNew.Application.Requests.Products;

public record RemoveProductRequest(ulong ServerId, ProductId ProductId) : IRequest;

public class RemoveProductRequestHandler(
    IProductRepository productRepository,
    ILogger<RemoveProductRequestHandler> logger) : IRequestHandler<RemoveProductRequest>
{
    public async Task Handle(RemoveProductRequest request, CancellationToken cancellationToken)
    {
        var product = await productRepository.GetByIdNoTrackingAsync(request.ProductId);
        if (product is null)
            return;
        
        var shopId = new ShopId(request.ServerId);
        if (product.ShopId != shopId)
            return;
        
        logger.LogInformation(string.Format(
            "Successfully removed product with id #{0}",
            product.Id.Value));
        
        await productRepository.Delete(request.ProductId);
    }
}