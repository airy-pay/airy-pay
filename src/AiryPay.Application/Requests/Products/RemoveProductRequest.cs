using AiryPay.Domain.Entities.Products;
using AiryPay.Domain.Entities.Shops;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AiryPay.Application.Requests.Products;

public record RemoveProductRequest(ShopId ShopId, ProductId ProductId) : IRequest;

public class RemoveProductRequestHandler(
    IProductRepository productRepository,
    ILogger<RemoveProductRequestHandler> logger) : IRequestHandler<RemoveProductRequest>
{
    public async Task Handle(RemoveProductRequest request, CancellationToken cancellationToken)
    {
        var product = await productRepository.GetByIdNoTrackingAsync(request.ProductId, cancellationToken);
        if (product is null)
            return;
        
        if (product.ShopId != request.ShopId)
            return;
        
        logger.LogInformation(string.Format(
            "Successfully removed product with id #{0}",
            product.Id.Value));
        
        await productRepository.DeleteAsync(request.ProductId, cancellationToken);
    }
}