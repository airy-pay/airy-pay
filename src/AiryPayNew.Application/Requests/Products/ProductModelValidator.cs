using FluentValidation;

namespace AiryPayNew.Application.Requests.Products;

public class ProductModelValidator : AbstractValidator<ProductModel>
{
    private const string DiscordEmoteRegex = @"(<a?)?:\w+:(\d{18}>)?";
    private const string EmojisRegex =
        @"(\u00a9|\u00ae|[\u2000-\u3300]|\ud83c[\ud000-\udfff]|\ud83d[\ud000-\udfff]|\ud83e[\ud000-\udfff])";
    
    public ProductModelValidator()
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