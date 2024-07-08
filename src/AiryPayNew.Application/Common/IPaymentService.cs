using AiryPayNew.Domain.Entities.Bills;

namespace AiryPayNew.Application.Common;

public interface IPaymentService
{
    public Task<string> CreateAsync(Bill bill, string paymentMethod);
}