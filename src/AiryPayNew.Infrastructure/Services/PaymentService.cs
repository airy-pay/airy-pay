using AiryPayNew.Application.Common;
using AiryPayNew.Domain.Entities.Bills;
using Ru.Kassa;
using Ru.Kassa.Models;
using Ru.Kassa.Models.Requests.Merchant;

namespace AiryPayNew.Infrastructure.Services;

public class PaymentService(RuKassaClient ruKassaClient) : IPaymentService
{
    public async Task<string> CreateAsync(Bill bill, string paymentMethod)
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
            throw new ArgumentException("RuKassa payment creation error: " + payment.Error);
        
        return payment.Url;
    }
}