using AiryPay.Application.Requests.Payments;
using AiryPay.Domain.Common.Result;
using AiryPay.Domain.Entities.Products;
using AiryPay.Domain.Entities.Shops;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AiryPay.Application.Requests.Products;

using RequestError = CreateProductRequest.Error;

public record CreateProductRequest(ShopId ShopId, ProductModel ProductModel)
    : IRequest<Result<CreateProductRequest.Error>>
{
    public enum Error
    {
        ValidationFailed,
        ShopNotFound,
        TooManyProductsCreated,
        ShopIsBlocked
    }
}

public class CreateProductRequestHandler(
    IProductRepository productRepository,
    IShopRepository shopRepository,
    IValidator<ProductModel> productValidator,
    ILogger<CreateProductRequestHandler> logger)
    : IRequestHandler<CreateProductRequest, Result<RequestError>>
{
    public async Task<Result<RequestError>> Handle(
        CreateProductRequest request, CancellationToken cancellationToken)
    {
        var resultBuilder = new ResultBuilder<RequestError>();
        
        var validationResult = await productValidator.ValidateAsync(request.ProductModel, cancellationToken);
        if (!validationResult.IsValid)
            return resultBuilder.WithError(RequestError.ValidationFailed);
        
        var shop = await shopRepository.GetByIdNoTrackingAsync(request.ShopId, cancellationToken);
        if (shop is null)
            return resultBuilder.WithError(RequestError.ShopNotFound);
        if (shop.Products.Count > 25)
            return resultBuilder.WithError(RequestError.TooManyProductsCreated);
        if (shop.Blocked)
            return resultBuilder.WithError(RequestError.ShopIsBlocked);
        
        var newProduct = new Product
        {
            Emoji = request.ProductModel.DiscordEmoji,
            Name = request.ProductModel.Name,
            Price = request.ProductModel.Price,
            DiscordRoleId = request.ProductModel.DiscordRoleId,
            ShopId = request.ShopId
        };
        
        newProduct.Id = await productRepository.CreateAsync(newProduct, cancellationToken);
        logger.LogInformation("Created product {ProductId} '{ProductName}' in shop {ShopId}.",
            newProduct.Id.Value, newProduct.Name, shop.Id.Value);
        
        return resultBuilder.WithSuccess();
    }
}