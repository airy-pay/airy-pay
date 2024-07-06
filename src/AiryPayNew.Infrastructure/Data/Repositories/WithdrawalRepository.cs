using AiryPayNew.Domain.Entities.Withdrawals;

namespace AiryPayNew.Infrastructure.Data.Repositories;

internal class WithdrawalRepository(ApplicationDbContext dbContext)
    : Repository<WithdrawalId, Withdrawal>(dbContext), IWithdrawalRepository
{
    private readonly ApplicationDbContext _dbContext = dbContext;

    public async Task UpdateStatus(WithdrawalId withdrawalId, WithdrawalStatus withdrawalStatus)
    {
        var withdrawal = await GetByIdAsync(withdrawalId);
        if (withdrawal is null)
            return;

        withdrawal.WithdrawalStatus = withdrawalStatus;
        await _dbContext.SaveChangesAsync();
    }
}