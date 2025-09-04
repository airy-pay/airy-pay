using AiryPay.Domain.Common.Result;
using AiryPay.Domain.Entities.Bills;

namespace AiryPay.Application.Payments;

public interface IPaymentService
{
    public string GetServiceName();
    public Task<Result<string, Error>> CreateAsync(Bill bill, string paymentMethod);
    
    public enum Error
    {
        Failed
    }
}