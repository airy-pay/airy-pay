using AiryPayNew.Domain.Common.Result;
using AiryPayNew.Domain.Entities.Products;
using AiryPayNew.Domain.Entities.Shops;
using FluentValidation;
using MediatR;

namespace AiryPayNew.Application.Requests.Products;

using Error = EditProductRequest.Error;

public record EditProductRequest(
    ShopId ShopId,
    ProductId ProductId,
    ProductModel ProductModel)
    : IRequest<Result<EditProductRequest.Error>>
{
    public enum Error
    {
        ValidationFailed,
        ShopNotFound,
        ShopIsBlocked,
        ProductNotFound,
        InvalidShopId
    }
}

public class EditProductRequestHandler(
    IShopRepository shopRepository,
    IProductRepository productRepository,
    IValidator<ProductModel> productValidator)
    : IRequestHandler<EditProductRequest, Result<Error>>
{
    public async Task<Result<Error>> Handle(
        EditProductRequest request, CancellationToken cancellationToken)
    {
        var resultBuilder = new ResultBuilder<Error>();
        
        var validationResult = await productValidator.ValidateAsync(request.ProductModel, cancellationToken);
        if (!validationResult.IsValid)
            return resultBuilder.WithError(Error.ValidationFailed);
        
        var shop = await shopRepository.GetByIdNoTrackingAsync(request.ShopId, cancellationToken);
        if (shop is null)
            return resultBuilder.WithError(Error.ShopNotFound);
        if (shop.Blocked)
            return resultBuilder.WithError(Error.ShopIsBlocked);
        
        var product = await productRepository.GetByIdNoTrackingAsync(request.ProductId, cancellationToken);
        if (product is null)
            return resultBuilder.WithError(Error.ProductNotFound);
        if (product.ShopId != request.ShopId)
            return resultBuilder.WithError(Error.InvalidShopId);
        
        await productRepository.UpdateAsync(
            request.ProductId,
            request.ProductModel.DiscordEmoji,
            request.ProductModel.Name,
            request.ProductModel.Price,
            request.ProductModel.DiscordRoleId,
            cancellationToken);
        
        return Result<Error>.Success();
    }
}