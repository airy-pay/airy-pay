using AiryPay.Domain.Entities.Bills;
using Microsoft.EntityFrameworkCore;

namespace AiryPay.Infrastructure.Data.Repositories;

internal class BillRepository(ApplicationDbContext dbContext)
    : Repository<BillId, Bill>(dbContext), IBillRepository
{
    private readonly ApplicationDbContext _dbContext = dbContext;

    public override async Task<Bill?> GetByIdAsync(BillId id, CancellationToken cancellationToken)
    {
        return await _dbContext.Bills
            .Include(x => x.Product)
            .Include(x => x.Shop)
            .Include(x => x.Purchase)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken: cancellationToken);
    }

    public override async Task<Bill?> GetByIdNoTrackingAsync(BillId id, CancellationToken cancellationToken)
    {
        return await _dbContext.Bills
            .AsNoTracking()
            .Include(x => x.Product)
            .Include(x => x.Shop)
            .Include(x => x.Purchase)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken: cancellationToken);
    }
    
    public async Task PayBillAsync(BillId billId, CancellationToken cancellationToken)
    {
        var bill = await GetByIdAsync(billId, cancellationToken);
        if (bill is null)
            return;

        bill.BillStatus = BillStatus.Paid;
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}