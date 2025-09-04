using AiryPay.Domain.Common;

namespace AiryPay.Domain.Entities.Products;

public record struct ProductId(long Value) : IIdBase<long>
{
    public static IId Default() => new ProductId(1);
}