using AiryPay.Domain.Common;

namespace AiryPay.Domain.Entities.Purchases;

public record struct PurchaseId(int Value) : IIdBase<int>
{
    public static IId Default() => new PurchaseId(1);
}