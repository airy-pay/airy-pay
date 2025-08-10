using AiryPayNew.Domain.Entities.Bills;
using MediatR;

namespace AiryPayNew.Application.Requests.Payments;

public record GetBillRequest(BillId BillId) : IRequest<Bill?>;

public class GetBillRequestHandler(IBillRepository billRepository)
    : IRequestHandler<GetBillRequest, Bill?>
{
    public async Task<Bill?> Handle(GetBillRequest request, CancellationToken cancellationToken)
    {
        var bill = await billRepository.GetByIdNoTrackingAsync(request.BillId, cancellationToken);
        return bill;
    }
}

