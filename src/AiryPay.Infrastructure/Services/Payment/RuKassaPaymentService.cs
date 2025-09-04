using AiryPay.Application.Payments;
using AiryPay.Domain.Common.Result;
using AiryPay.Domain.Entities.Bills;
using Ru.Kassa;
using Ru.Kassa.Models;
using Ru.Kassa.Models.Requests.Merchant;

namespace AiryPay.Infrastructure.Services.Payment;

public class RuKassaPaymentService(RuKassaClient ruKassaClient) : IPaymentService
{
    private const string Name = "RuKassa";

    public string GetServiceName() => Name;

    public async Task<Result<string, IPaymentService.Error>> CreateAsync(Bill bill, string paymentMethod)
    {
        var payment = await ruKassaClient.CreatePaymentAsync(
            new PaymentMerchantRequest
            {
                OrderId = bill.Id.Value,
                Amount = bill.Product.Price,
                Data = bill.BillSecret.Key,
                Method = paymentMethod,
                Currency = Currency.RUB
            });

        if (!string.IsNullOrEmpty(payment.Error))
            return Result<string, IPaymentService.Error>.Fail(
                string.Empty, 
                IPaymentService.Error.Failed);
        
        return Result<string, IPaymentService.Error>.Success(payment.Url);
    }
}