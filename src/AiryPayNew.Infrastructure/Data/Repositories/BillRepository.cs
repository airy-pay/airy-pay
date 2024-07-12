using AiryPayNew.Domain.Entities.Bills;
using Microsoft.EntityFrameworkCore;

namespace AiryPayNew.Infrastructure.Data.Repositories;

internal class BillRepository(ApplicationDbContext dbContext)
    : Repository<BillId, Bill>(dbContext), IBillRepository
{
    private readonly ApplicationDbContext _dbContext = dbContext;

    public override async Task<Bill?> GetByIdAsync(BillId id)
    {
        return await _dbContext.Bills
            .Include(x => x.Product)
            .Include(x => x.Shop)
            .Include(x => x.Purchase)
            .FirstOrDefaultAsync(x => x.Id == id);
    }
    
    public async Task PayBill(BillId billId)
    {
        var bill = await GetByIdAsync(billId);
        if (bill is null)
            return;

        bill.BillStatus = BillStatus.Paid;
        await _dbContext.SaveChangesAsync();
    }
}