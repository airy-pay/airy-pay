using AiryPayNew.Domain.Common.Repositories;

namespace AiryPayNew.Domain.Entities.Withdrawals;

public interface IWithdrawalRepository : IDefaultRepository<WithdrawalId, Withdrawal>
{
    public Task UpdateStatusAsync(
        WithdrawalId withdrawalId,
        WithdrawalStatus withdrawalStatus,
        CancellationToken cancellationToken);
}