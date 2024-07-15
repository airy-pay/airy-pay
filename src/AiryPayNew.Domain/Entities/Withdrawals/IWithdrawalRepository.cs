using AiryPayNew.Domain.Common.Repositories;

namespace AiryPayNew.Domain.Entities.Withdrawals;

public interface IWithdrawalRepository : IDefaultRepository<WithdrawalId, Withdrawal>
{
    public Task UpdateStatus(WithdrawalId withdrawalId, WithdrawalStatus withdrawalStatus);
}