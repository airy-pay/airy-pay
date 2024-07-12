using AiryPayNew.Domain.Common;
using AiryPayNew.Domain.Entities.Bills;
using AiryPayNew.Domain.Entities.Purchases;
using AiryPayNew.Domain.Entities.Shops;
using MediatR;

namespace AiryPayNew.Application.Requests.Payments;

public record CompletePaymentRequest(BillId BillId) : IRequest<OperationResult>;

public class CompletePaymentRequestHandler(
    IBillRepository billRepository,
    IPurchaseRepository purchaseRepository,
    IShopRepository shopRepository) : IRequestHandler<CompletePaymentRequest, OperationResult>
{
    public async Task<OperationResult> Handle(CompletePaymentRequest request, CancellationToken cancellationToken)
    {
        var bill = await billRepository.GetByIdAsync(request.BillId);
        if (bill is null)
            return OperationResult.Error("Bill not found");

        await billRepository.PayBill(bill.Id);

        var newPurchase = new Purchase
        {
            DateTime = DateTime.UtcNow,
            ProductId = bill.ProductId,
            ShopId = bill.ShopId,
            BillId = bill.Id
        };
        await purchaseRepository.Create(newPurchase);

        var shopBalanceChange = bill.Product.Price * 0.9m;
        await shopRepository.UpdateBalance(bill.ShopId, shopBalanceChange);
        
        return OperationResult.Success();
    }
}