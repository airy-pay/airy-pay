using AiryPayNew.Application.Requests.Payments;
using AiryPayNew.Domain.Common.Result;
using AiryPayNew.Domain.Entities.Products;
using AiryPayNew.Domain.Entities.Shops;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AiryPayNew.Application.Requests.Products;

using RequestError = CreateProductRequest.Error;

public record CreateProductRequest(ulong ShopId, ProductModel ProductModel)
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
    ILogger<CreatePaymentRequestHandler> logger)
    : IRequestHandler<CreateProductRequest, Result<RequestError>>
{
    public async Task<Result<RequestError>> Handle(
        CreateProductRequest request, CancellationToken cancellationToken)
    {
        var resultBuilder = new ResultBuilder<RequestError>();
        
        var validationResult = await productValidator.ValidateAsync(request.ProductModel, cancellationToken);
        if (!validationResult.IsValid)
            return resultBuilder.WithError(RequestError.ValidationFailed);

        var shopId = new ShopId(request.ShopId);
        var shop = await shopRepository.GetByIdNoTrackingAsync(shopId, cancellationToken);
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
            ShopId = shopId
        };
        
        newProduct.Id = await productRepository.CreateAsync(newProduct, cancellationToken);
        logger.LogInformation(string.Format(
            "Created a new product with id #{0} in shop #{1}",
            newProduct.Id.Value,
            shop.Id.Value));
        
        return resultBuilder.WithSuccess();
    }
}