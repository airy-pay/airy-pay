using AiryPayNew.Domain.Common;
using AiryPayNew.Domain.Entities.Bills;
using AiryPayNew.Domain.Entities.Purchases;
using AiryPayNew.Domain.Entities.Shops;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AiryPayNew.Application.Requests.Payments;

public record CompletePaymentRequest(BillId BillId) : IRequest<OperationResult>;

public class CompletePaymentRequestHandler(
    IBillRepository billRepository,
    IPurchaseRepository purchaseRepository,
    IShopRepository shopRepository,
    ILogger<CompletePaymentRequestHandler> logger) : IRequestHandler<CompletePaymentRequest, OperationResult>
{
    public async Task<OperationResult> Handle(CompletePaymentRequest request, CancellationToken cancellationToken)
    {
        var bill = await billRepository.GetByIdNoTrackingAsync(request.BillId, cancellationToken);
        if (bill is null)
        {
            logger.LogInformation(
                string.Format("Tried to complete payment, but bill with id {0} was not found.",
                request.BillId.Value));
            return OperationResult.Error("Bill not found");
        }

        await billRepository.PayBillAsync(bill.Id, cancellationToken);

        var newPurchase = new Purchase
        {
            DateTime = DateTime.UtcNow,
            ProductId = bill.ProductId,
            ShopId = bill.ShopId,
            BillId = bill.Id
        };
        var purchaseId = await purchaseRepository.CreateAsync(newPurchase, cancellationToken);

        var commissionMultiplier = 1m - (bill.Shop.Commission.Value / 100);
        var shopBalanceChange = bill.Product.Price * commissionMultiplier;
        await shopRepository.UpdateBalanceAsync(bill.ShopId, shopBalanceChange, cancellationToken);
        
        logger.LogInformation(
            string.Format("Successfully completed a payment for bill with id {0}. New purchase id: {1}",
                request.BillId.Value,
                purchaseId.Value));
        
        return OperationResult.Success();
    }
}