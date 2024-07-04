using AiryPayNew.Domain.Entities.Bills;

namespace AiryPayNew.Infrastructure.Data.Repositories;

public class BillRepository(ApplicationDbContext dbContext)
    : Repository<BillId, Bill>(dbContext), IBillRepository
{
    private readonly ApplicationDbContext _dbContext1 = dbContext;

    public async Task PayBill(BillId billId)
    {
        var bill = await GetByIdAsync(billId);
        if (bill is null)
            return;

        bill.BillStatus = BillStatus.Paid;
        await _dbContext1.SaveChangesAsync();
    }
}