using AiryPay.Domain.Common;

namespace AiryPay.Domain.Entities.Shops;

public record struct ShopId(long Value) : IIdBase<long>
{
    public static IId Default() => new ShopId(1);

    public ShopId(ulong Value) : this((long) Value) { }
}