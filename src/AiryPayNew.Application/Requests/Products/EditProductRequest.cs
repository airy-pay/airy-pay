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
        var shop = await shopRepository.GetByIdAsync(shopId);
        if (shop is null)
            return OperationResult.Error("Магазин не найден.");
        
        var product = await productRepository.GetByIdAsync(request.ProductId);
        if (product is null)
            return OperationResult.Error("Товар не найден.");
        if (product.ShopId != shopId)
            return OperationResult.Error("Неверный Id магазина.");
        
        await productRepository.Update(
            request.ProductId,
            request.ProductModel.DiscordEmoji,
            request.ProductModel.Name,
            request.ProductModel.Price,
            request.ProductModel.DiscordRoleId);
        return OperationResult.Success();
    }
}