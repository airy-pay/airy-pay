using AiryPay.Application.Payments;
using AiryPay.Domain.Common.Result;
using AiryPay.Domain.Entities.Bills;
using AiryPay.Domain.Entities.Products;
using AiryPay.Domain.Entities.Shops;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AiryPay.Application.Requests.Payments;

using RequestError = CreatePaymentRequest.Error;

public record CreatePaymentRequest(
    ProductId ProductId,
    string PaymentServiceName,
    string PaymentMethodId,
    ulong BuyerId,
    ulong ShopId)
    : IRequest<Result<string, CreatePaymentRequest.Error>>
{
    public enum Error
    {
        ShopNotFound,
        ShopIsBlocked,
        PaymentServiceNotFound,
        ProductNotFound,
        AccessDenied,
        FailedToCreate
    }
}

public class CreatePaymentRequestHandler(
    IBillRepository billRepository,
    IProductRepository productRepository,
    IShopRepository shopRepository,
    IEnumerable<IPaymentService> paymentServices,
    ILogger<CreatePaymentRequestHandler> logger)
    : IRequestHandler<CreatePaymentRequest, Result<string, RequestError>>
{
    public async Task<Result<string, RequestError>> Handle(
        CreatePaymentRequest request, CancellationToken cancellationToken)
    {
        var resultBuilder = new ResultBuilder<string, RequestError>(string.Empty);
        
        var shop = await shopRepository.GetByIdNoTrackingAsync(new ShopId(request.ShopId), cancellationToken);
        if (shop is null)
            return resultBuilder.WithError(RequestError.ShopNotFound);
        if (shop.Blocked)
            return resultBuilder.WithError(RequestError.ShopIsBlocked);
        
        var paymentService = paymentServices.FirstOrDefault(x =>
            x.GetServiceName() == request.PaymentServiceName);
        if (paymentService is null)
            return resultBuilder.WithError(RequestError.PaymentServiceNotFound);
        
        var product = await productRepository.GetByIdNoTrackingAsync(request.ProductId, cancellationToken);
        if (product is null)
            return resultBuilder.WithError(RequestError.ProductNotFound);
        if (product.ShopId != shop.Id)
            return resultBuilder.WithError(RequestError.AccessDenied);
        
        var newBill = new Bill
        {
            BillStatus = BillStatus.Unpaid,
            BuyerDiscordId = request.BuyerId,
            ProductId = product.Id,
            ShopId = shop.Id,
            Payment = new Payment()
        };
        
        newBill.Id = await billRepository.CreateAsync(newBill, cancellationToken);

        var bill = await billRepository.GetByIdNoTrackingAsync(newBill.Id, cancellationToken);
        if (bill is null)
            return resultBuilder.WithError(RequestError.FailedToCreate);
        
        var paymentUrlResult = await paymentService.CreateAsync(bill, request.PaymentMethodId);

        logger.LogInformation(string.Format(
            "Successfully created a new payment for bill #{0}",
            newBill.Id));
        
        return Result<string, RequestError>.Success(paymentUrlResult.Entity);
    }
}