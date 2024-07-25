using AiryPayNew.Domain.Entities.Bills;
using MediatR;

namespace AiryPayNew.Application.Requests.Payments;

public record GetBillRequest(int BillId) : IRequest<Bill?>;

public class GetBillRequestHandler(IBillRepository billRepository) : IRequestHandler<GetBillRequest, Bill?>
{
    public async Task<Bill?> Handle(GetBillRequest request, CancellationToken cancellationToken)
    {
        var billId = new BillId(request.BillId);
        var bill = await billRepository.GetByIdNoTrackingAsync(billId, cancellationToken);
        return bill;
    }
}

