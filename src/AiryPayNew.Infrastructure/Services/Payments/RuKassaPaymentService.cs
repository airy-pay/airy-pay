using AiryPayNew.Application.Payments;
using AiryPayNew.Domain.Common;
using AiryPayNew.Domain.Entities.Bills;
using AiryPayNew.Domain.Entities.Bills.BillSecrets;
using AiryPayNew.Domain.Entities.Bills.BillSecrets.BillSecretGenerators;
using Ru.Kassa;
using Ru.Kassa.Models;
using Ru.Kassa.Models.Requests.Merchant;

namespace AiryPayNew.Infrastructure.Services.Payments;

public class RuKassaPaymentService(RuKassaClient ruKassaClient) : IPaymentService
{
    private const string Name = "RuKassa";

    public string GetServiceName() => Name;

    public async Task<OperationResult<string>> CreateAsync(Bill bill, string paymentMethod)
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
            return OperationResult<string>.Error(
                string.Empty, 
                "RuKassa payment creation error: " + payment.Error);
        
        return OperationResult<string>.Success(payment.Url);
    }
}