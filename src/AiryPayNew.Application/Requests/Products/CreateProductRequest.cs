using AiryPayNew.Domain.Common;
using AiryPayNew.Domain.Entities.Products;
using AiryPayNew.Domain.Entities.Shops;
using FluentValidation;
using MediatR;

namespace AiryPayNew.Application.Requests.Products;

public record CreateProductRequest(
    ulong ShopId, string DiscordEmoji, string Name, decimal Price) : IRequest<OperationResult>;

public class CreateProductRequestValidator : AbstractValidator<CreateProductRequest>
{
    private const string DiscordEmoteRegex = @"(<a?)?:\w+:(\d{18}>)?";
    private const string EmojisRegex =
        @"(\u00a9|\u00ae|[\u2000-\u3300]|\ud83c[\ud000-\udfff]|\ud83d[\ud000-\udfff]|\ud83e[\ud000-\udfff])";
    
    public CreateProductRequestValidator()
    {
        RuleFor(x => x.DiscordEmoji)
            .MinimumLength(1)
            .MaximumLength(64)
            .Matches(DiscordEmoteRegex + "|" + EmojisRegex)
            .NotNull()
            .WithName("Emoji");
        RuleFor(x => x.Name)
            .MinimumLength(3)
            .MaximumLength(32)
            .Matches(@"^[\p{L}\p{N}\s]+$")
            .NotNull()
            .WithName("Название");
        RuleFor(x => x.Price)
            .GreaterThanOrEqualTo(50)
            .LessThanOrEqualTo(10000)
            .WithName("Цена");
    }
}

public class CreateProductRequestHandler(
    IProductRepository productRepository,
    IShopRepository shopRepository,
    IValidator<CreateProductRequest> requestValidator) : IRequestHandler<CreateProductRequest, OperationResult>
{
    public async Task<OperationResult> Handle(CreateProductRequest request, CancellationToken cancellationToken)
    {
        var validationResult = await requestValidator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
            return OperationResult.Error(validationResult.Errors.First().ToString());

        var shopId = new ShopId(request.ShopId);
        var shop = await shopRepository.GetByIdAsync(shopId);
        if (shop is null)
            return OperationResult.Error("Магазин не найден.");
        if (shop.Products.Count > 25)
            return OperationResult.Error("Количество товаров не может быть больше 25.");
        
        var newProduct = new Product
        {
            Emoji = request.DiscordEmoji,
            Name = request.Name,
            Price = request.Price,
            ShopId = shopId
        };

        await productRepository.Create(newProduct);
        return OperationResult.Success();
    }
}