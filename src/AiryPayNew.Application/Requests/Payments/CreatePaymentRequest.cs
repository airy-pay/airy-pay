using AiryPayNew.Application.Common;
using AiryPayNew.Domain.Common;
using AiryPayNew.Domain.Entities.Bills;
using AiryPayNew.Domain.Entities.Products;
using AiryPayNew.Domain.Entities.Shops;
using MediatR;

namespace AiryPayNew.Application.Requests.Payments;

public record CreatePaymentRequest(
    long ProductId, string PaymentMethod, ulong BuyerId, ulong ShopId) : IRequest<OperationResult<string>>;

public class CreatePaymentRequestHandler(
    IPaymentService paymentService,
    IBillRepository billRepository,
    IProductRepository productRepository,
    IShopRepository shopRepository) : IRequestHandler<CreatePaymentRequest, OperationResult<string>>
{
    public async Task<OperationResult<string>> Handle(CreatePaymentRequest request, CancellationToken cancellationToken)
    {
        var shop = await shopRepository.GetByIdAsync(new ShopId(request.ShopId));
        if (shop is null)
            return Error("Магазин не найден.");
        if (shop.Blocked)
            return Error("Магазин заблокирован.");
        
        var product = await productRepository.GetByIdAsync(new ProductId(request.ProductId));
        if (product is null)
            return Error("Товар не найден.");
        if (product.ShopId != shop.Id)
            return Error("Нет доступа.");
        
        var newBill = new Bill
        {
            BillStatus = BillStatus.Unpaid,
            BuyerDiscordId = request.BuyerId,
            ProductId = product.Id,
            ShopId = shop.Id,
            Product = product
        };
        
        var paymentUrl = await paymentService.CreateAsync(newBill, request.PaymentMethod);
        await billRepository.Create(newBill);

        return OperationResult<string>.Success(paymentUrl);
    }

    private static OperationResult<string> Error(string message)
    {
        return OperationResult<string>.Error(string.Empty, message);
    }
}