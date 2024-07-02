using AiryPayNew.Domain.Common;

namespace AiryPayNew.Domain.Entities.Withdrawals;

public record struct WithdrawalId(int Value) : IIdBase<int>;