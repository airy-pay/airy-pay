using AiryPayNew.Domain.Common;
using AiryPayNew.Domain.Entities.Products;
using AiryPayNew.Domain.Entities.Shops;
using FluentValidation;
using MediatR;

namespace AiryPayNew.Application.Requests.Products;

public record EditProductRequest(
    ulong ShopId, ProductId ProductId, ProductModel ProductModel) : IRequest<OperationResult>;

public class EditProductRequestHandler(
    IShopRepository shopRepository,
    IProductRepository productRepository,
    IValidator<ProductModel> productValidator) : IRequestHandler<EditProductRequest, OperationResult>
{
    public async Task<OperationResult> Handle(EditProductRequest request, CancellationToken cancellationToken)
    {
        var validationResult = await productValidator.ValidateAsync(request.ProductModel, cancellationToken);
        if (!validationResult.IsValid)
            return OperationResult.Error(validationResult.Errors.First().ToString());

        var shopId = new ShopId(request.ShopId);
        var shop = await shopRepository.GetByIdNoTrackingAsync(shopId, cancellationToken);
        if (shop is null)
            return OperationResult.Error("Shop not found.");
        if (shop.Blocked)
            return OperationResult.Error("Shop is blocked.");
        
        var product = await productRepository.GetByIdNoTrackingAsync(request.ProductId, cancellationToken);
        if (product is null)
            return OperationResult.Error("Product not found.");
        if (product.ShopId != shopId)
            return OperationResult.Error("Invalid shop Id.");
        
        await productRepository.UpdateAsync(
            request.ProductId,
            request.ProductModel.DiscordEmoji,
            request.ProductModel.Name,
            request.ProductModel.Price,
            request.ProductModel.DiscordRoleId,
            cancellationToken);
        return OperationResult.Success();
    }
}