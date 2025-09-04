using AiryPay.Domain.Common;

namespace AiryPay.Domain.Entities.Bills;

public record struct BillId(int Value) : IIdBase<int>
{
    public static IId Default() => new BillId(1);
}