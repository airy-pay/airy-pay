using AiryPay.Domain.Common.Repositories;

namespace AiryPay.Domain.Entities.Withdrawals;

public interface IWithdrawalRepository : IDefaultRepository<WithdrawalId, Withdrawal>
{
    public Task UpdateStatusAsync(
        WithdrawalId withdrawalId,
        WithdrawalStatus withdrawalStatus,
        CancellationToken cancellationToken);
}