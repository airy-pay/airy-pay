using AiryPayNew.Domain.Common;

namespace AiryPayNew.Domain.Entities.Withdrawals;

public interface IWithdrawalRepository : IRepository<WithdrawalId, Withdrawal>
{
    public Task UpdateStatus(WithdrawalId withdrawalId, WithdrawalStatus withdrawalStatus);
}