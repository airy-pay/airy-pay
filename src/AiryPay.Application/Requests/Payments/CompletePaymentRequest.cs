﻿using AiryPay.Domain.Common.Result;
using AiryPay.Domain.Entities.Bills;
using AiryPay.Domain.Entities.Purchases;
using AiryPay.Domain.Entities.Shops;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AiryPay.Application.Requests.Payments;

public record CompletePaymentRequest(BillId BillId)
    : IRequest<Result<CompletePaymentRequest.Error>>
{
    public enum Error
    {
        BillNotFound
    }
}

public class CompletePaymentRequestHandler(
    IBillRepository billRepository,
    IPurchaseRepository purchaseRepository,
    IShopRepository shopRepository,
    ILogger<CompletePaymentRequestHandler> logger)
    : IRequestHandler<CompletePaymentRequest, Result<CompletePaymentRequest.Error>>
{
    public async Task<Result<CompletePaymentRequest.Error>> Handle(
        CompletePaymentRequest request, CancellationToken cancellationToken)
    {
        var bill = await billRepository.GetByIdNoTrackingAsync(request.BillId, cancellationToken);
        if (bill is null)
        {
            logger.LogInformation(
                string.Format("Tried to complete payment, but bill with id {0} was not found.",
                request.BillId.Value));
            
            return Result<CompletePaymentRequest.Error>.Fail(CompletePaymentRequest.Error.BillNotFound);
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
        
        return Result<CompletePaymentRequest.Error>.Success();
    }
}