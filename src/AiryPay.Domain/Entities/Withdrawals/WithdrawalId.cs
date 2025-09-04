using AiryPay.Domain.Common;

namespace AiryPay.Domain.Entities.Withdrawals;

public record struct WithdrawalId(int Value) : IIdBase<int>
{
    public static IId Default() => new WithdrawalId(1);
}