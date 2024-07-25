using AiryPayNew.Domain.Entities.Withdrawals;

namespace AiryPayNew.Infrastructure.Data.Repositories;

internal class WithdrawalRepository(ApplicationDbContext dbContext)
    : Repository<WithdrawalId, Withdrawal>(dbContext), IWithdrawalRepository
{
    private readonly ApplicationDbContext _dbContext = dbContext;

    public async Task UpdateStatusAsync(
        WithdrawalId withdrawalId,
        WithdrawalStatus withdrawalStatus,
        CancellationToken cancellationToken)
    {
        var withdrawal = await GetByIdAsync(withdrawalId, cancellationToken);
        if (withdrawal is null)
            return;

        withdrawal.WithdrawalStatus = withdrawalStatus;
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}