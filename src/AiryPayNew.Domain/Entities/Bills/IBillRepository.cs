using AiryPayNew.Domain.Common;

namespace AiryPayNew.Domain.Entities.Bills;

public interface IBillRepository : IRepository<BillId, Bill>
{
    public Task PayBill(BillId billId);
}