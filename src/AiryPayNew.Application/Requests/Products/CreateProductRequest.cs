using AiryPayNew.Application.Requests.Payments;
using AiryPayNew.Domain.Common;
using AiryPayNew.Domain.Entities.Products;
using AiryPayNew.Domain.Entities.Shops;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AiryPayNew.Application.Requests.Products;

public record CreateProductRequest(ulong ShopId, ProductModel ProductModel) : IRequest<OperationResult>;

public class CreateProductRequestHandler(
    IProductRepository productRepository,
    IShopRepository shopRepository,
    IValidator<ProductModel> productValidator,
    ILogger<CreatePaymentRequestHandler> logger) : IRequestHandler<CreateProductRequest, OperationResult>
{
    public async Task<OperationResult> Handle(CreateProductRequest request, CancellationToken cancellationToken)
    {
        var validationResult = await productValidator.ValidateAsync(request.ProductModel, cancellationToken);
        if (!validationResult.IsValid)
            return OperationResult.Error(validationResult.Errors.First().ToString());

        var shopId = new ShopId(request.ShopId);
        var shop = await shopRepository.GetByIdNoTrackingAsync(shopId, cancellationToken);
        if (shop is null)
            return OperationResult.Error("Shop not found.");
        if (shop.Products.Count > 25)
            return OperationResult.Error("The number of products cannot exceed 25.");
        if (shop.Blocked)
            return OperationResult.Error("Shop is blocked.");
        
        var newProduct = new Product
        {
            Emoji = request.ProductModel.DiscordEmoji,
            Name = request.ProductModel.Name,
            Price = request.ProductModel.Price,
            DiscordRoleId = request.ProductModel.DiscordRoleId,
            ShopId = shopId
        };
        
        newProduct.Id = await productRepository.CreateAsync(newProduct, cancellationToken);
        logger.LogInformation(string.Format(
            "Created a new product with id #{0} in shop #{1}",
            newProduct.Id.Value,
            shop.Id.Value));
        
        return OperationResult.Success();
    }
}