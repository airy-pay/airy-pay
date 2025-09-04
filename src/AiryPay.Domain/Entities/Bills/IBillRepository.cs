using AiryPay.Domain.Common.Repositories;

namespace AiryPay.Domain.Entities.Bills;

public interface IBillRepository :
    IDefaultRepository<BillId, Bill>, INoTrackRepository<BillId, Bill>
{
    public Task PayBillAsync(BillId billId, CancellationToken cancellationToken);
}