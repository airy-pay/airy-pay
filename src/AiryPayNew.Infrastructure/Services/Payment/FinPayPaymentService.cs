using AiryPayNew.Application.Payments;
using AiryPayNew.Domain.Common.Result;
using AiryPayNew.Domain.Entities.Bills;
using FinPay.API;
using FinPay.API.Requests.Impl;

namespace AiryPayNew.Infrastructure.Services.Payment;

public class FinPayPaymentService(FinPayApiClient finPayApiClient) : IPaymentService
{
    private const string Name = "FinPay"; 

    public string GetServiceName() => Name; 
    
    public async Task<Result<string, IPaymentService.Error>> CreateAsync(Bill bill, string paymentMethod)
    {
        var payment = new CreatePaymentRequest
        {
            Amount = Convert.ToInt32(bill.Product.Price) * 100,
            CountryCode = "RU",
            Currency = "RUB",
            Description = "AiryPay ",
            InvoiceId = bill.Id.Value + 100000,
            PaymentMethod = paymentMethod
        };

        var result = await finPayApiClient.CreatePaymentAsync(payment);
        if (!result.Success)
        {
            return Result<string, IPaymentService.Error>.Fail("", IPaymentService.Error.Failed);
        }

        return Result<string, IPaymentService.Error>.Success(result.Url);
    }
}