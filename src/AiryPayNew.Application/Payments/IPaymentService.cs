using AiryPayNew.Domain.Common.Result;
using AiryPayNew.Domain.Entities.Bills;

namespace AiryPayNew.Application.Payments;

public interface IPaymentService
{
    public string GetServiceName();
    public Task<Result<string, Error>> CreateAsync(Bill bill, string paymentMethod);
    
    public enum Error
    {
        Failed
    }
}