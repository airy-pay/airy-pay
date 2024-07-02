using AiryPayNew.Domain.Common;

namespace AiryPayNew.Domain.Entities.Bills;

public record struct BillId(int Value) : IIdBase<int>
{
    public static IId Default() => new BillId(1);
}