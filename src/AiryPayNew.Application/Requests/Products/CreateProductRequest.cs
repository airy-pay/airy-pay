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
        var shop = await shopRepository.GetByIdAsync(shopId);
        if (shop is null)
            return OperationResult.Error("Магазин не найден.");
        if (shop.Products.Count > 25)
            return OperationResult.Error("Количество товаров не может быть больше 25.");
        if (shop.Blocked)
            return OperationResult.Error("Магазин заблокирован.");
        
        var newProduct = new Product
        {
            Emoji = request.ProductModel.DiscordEmoji,
            Name = request.ProductModel.Name,
            Price = request.ProductModel.Price,
            DiscordRoleId = request.ProductModel.DiscordRoleId,
            ShopId = shopId
        };
        
        newProduct.Id = await productRepository.Create(newProduct);
        logger.LogInformation(string.Format(
            "Created a new product with id #{0}",
            newProduct.Id.Value));
        
        return OperationResult.Success();
    }
}