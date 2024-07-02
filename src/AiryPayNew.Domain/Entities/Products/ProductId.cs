using AiryPayNew.Domain.Common;

namespace AiryPayNew.Domain.Entities.Products;

public record struct ProductId(long Value) : IIdBase<long>
{
    public static IId Default() => new ProductId(1);
}