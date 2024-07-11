using AiryPayNew.Domain.Common;
using AiryPayNew.Domain.Entities.Bills;

namespace AiryPayNew.Application.Payments;

public interface IPaymentService
{
    public string GetServiceName();
    public Task<OperationResult<string>> CreateAsync(Bill bill, string paymentMethod);
}