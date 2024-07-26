using AiryPayNew.Domain.Common.Repositories;

namespace AiryPayNew.Domain.Entities.Bills;

public interface IBillRepository :
    IDefaultRepository<BillId, Bill>, INoTrackRepository<BillId, Bill>
{
    public Task PayBillAsync(BillId billId, CancellationToken cancellationToken);
}