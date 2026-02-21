using AiryPay.Application.Requests.Payments;
using AiryPay.Domain.Entities.Bills;
using AiryPay.Domain.Entities.Products;
using AiryPay.Domain.Entities.Purchases;
using AiryPay.Domain.Entities.Shops;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace AiryPay.Tests.Application.Requests.Payments;

public class CompletePaymentRequestHandlerTests
{
    private readonly Mock<IBillRepository> _mockBillRepository;
    private readonly Mock<IPurchaseRepository> _mockPurchaseRepository;
    private readonly Mock<IShopRepository> _mockShopRepository;
    private readonly Mock<ILogger<CompletePaymentRequestHandler>> _mockLogger;
    private readonly CompletePaymentRequestHandler _handler;

    public CompletePaymentRequestHandlerTests()
    {
        _mockBillRepository = new Mock<IBillRepository>();
        _mockPurchaseRepository = new Mock<IPurchaseRepository>();
        _mockShopRepository = new Mock<IShopRepository>();
        _mockLogger = new Mock<ILogger<CompletePaymentRequestHandler>>();
        _handler = new CompletePaymentRequestHandler(
            _mockBillRepository.Object,
            _mockPurchaseRepository.Object,
            _mockShopRepository.Object,
            _mockLogger.Object);
    }

    [Fact]
    public async Task Handle_WhenBillNotFound_ShouldReturnBillNotFoundError()
    {
        var request = new CompletePaymentRequest(new BillId(1));
        _mockBillRepository
            .Setup(r => r.GetByIdNoTrackingAsync(It.IsAny<BillId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Bill?)null);

        var result = await _handler.Handle(request, CancellationToken.None);

        result.Successful.Should().BeFalse();
        result.ErrorType.Should().Be(CompletePaymentRequest.Error.BillNotFound);
    }

    [Fact]
    public async Task Handle_WhenBillExists_ShouldPayBillCreatePurchaseAndUpdateBalance()
    {
        var billId = new BillId(1);
        var shopId = new ShopId(1);
        var productId = new ProductId(1);
        var shop = new Shop { Id = shopId, Commission = new Commission(10m) };
        var product = new Product { Id = productId, Price = 1000m };
        var bill = new Bill
        {
            Id = billId,
            ShopId = shopId,
            ProductId = productId,
            Shop = shop,
            Product = product,
            Payment = new Payment()
        };
        var request = new CompletePaymentRequest(billId);
        _mockBillRepository
            .Setup(r => r.GetByIdNoTrackingAsync(billId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(bill);
        _mockPurchaseRepository
            .Setup(r => r.CreateAsync(It.IsAny<Purchase>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PurchaseId(1));

        var result = await _handler.Handle(request, CancellationToken.None);

        result.Successful.Should().BeTrue();
        _mockBillRepository.Verify(
            r => r.PayBillAsync(billId, It.IsAny<CancellationToken>()),
            Times.Once);
        _mockPurchaseRepository.Verify(
            r => r.CreateAsync(It.Is<Purchase>(p =>
                p.BillId == billId &&
                p.ProductId == productId &&
                p.ShopId == shopId),
                It.IsAny<CancellationToken>()),
            Times.Once);
        // commission 10% => multiplier 0.9 => shop gets 900
        _mockShopRepository.Verify(
            r => r.UpdateBalanceAsync(shopId, 900m, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Theory]
    [InlineData(0, 1000, 1000)]    // 0% commission: shop gets full price
    [InlineData(10, 1000, 900)]    // 10% commission: shop gets 90%
    [InlineData(25, 1000, 750)]    // 25% commission: shop gets 75%
    [InlineData(50, 1000, 500)]    // 50% commission: shop gets 50%
    [InlineData(100, 1000, 0)]     // 100% commission: shop gets nothing
    public async Task Handle_ShouldApplyCorrectCommissionToBalanceChange(
        decimal commissionPercent, decimal productPrice, decimal expectedBalanceChange)
    {
        var billId = new BillId(1);
        var shopId = new ShopId(1);
        var productId = new ProductId(1);
        var shop = new Shop { Id = shopId, Commission = new Commission(commissionPercent) };
        var product = new Product { Id = productId, Price = productPrice };
        var bill = new Bill
        {
            Id = billId,
            ShopId = shopId,
            ProductId = productId,
            Shop = shop,
            Product = product,
            Payment = new Payment()
        };
        var request = new CompletePaymentRequest(billId);
        _mockBillRepository
            .Setup(r => r.GetByIdNoTrackingAsync(billId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(bill);
        _mockPurchaseRepository
            .Setup(r => r.CreateAsync(It.IsAny<Purchase>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PurchaseId(1));

        await _handler.Handle(request, CancellationToken.None);

        _mockShopRepository.Verify(
            r => r.UpdateBalanceAsync(shopId, expectedBalanceChange, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WithFractionalCommission_ShouldCalculateBalanceCorrectly()
    {
        // 10.5% commission => multiplier 0.895 => 1000 * 0.895 = 895
        var billId = new BillId(1);
        var shopId = new ShopId(1);
        var productId = new ProductId(1);
        var shop = new Shop { Id = shopId, Commission = new Commission(10.5m) };
        var product = new Product { Id = productId, Price = 1000m };
        var bill = new Bill
        {
            Id = billId,
            ShopId = shopId,
            ProductId = productId,
            Shop = shop,
            Product = product,
            Payment = new Payment()
        };
        var request = new CompletePaymentRequest(billId);
        _mockBillRepository
            .Setup(r => r.GetByIdNoTrackingAsync(billId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(bill);
        _mockPurchaseRepository
            .Setup(r => r.CreateAsync(It.IsAny<Purchase>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PurchaseId(1));

        await _handler.Handle(request, CancellationToken.None);

        var expectedBalanceChange = 1000m * (1m - 10.5m / 100m); // 895
        _mockShopRepository.Verify(
            r => r.UpdateBalanceAsync(shopId, expectedBalanceChange, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WithDecimalProductPrice_ShouldPreservePrecision()
    {
        // 100.50 price, 10% commission => 90.45
        var billId = new BillId(1);
        var shopId = new ShopId(1);
        var productId = new ProductId(1);
        var shop = new Shop { Id = shopId, Commission = new Commission(10m) };
        var product = new Product { Id = productId, Price = 100.50m };
        var bill = new Bill
        {
            Id = billId,
            ShopId = shopId,
            ProductId = productId,
            Shop = shop,
            Product = product,
            Payment = new Payment()
        };
        var request = new CompletePaymentRequest(billId);
        _mockBillRepository
            .Setup(r => r.GetByIdNoTrackingAsync(billId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(bill);
        _mockPurchaseRepository
            .Setup(r => r.CreateAsync(It.IsAny<Purchase>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PurchaseId(1));

        await _handler.Handle(request, CancellationToken.None);

        var expectedBalanceChange = 100.50m * 0.9m; // 90.45
        _mockShopRepository.Verify(
            r => r.UpdateBalanceAsync(shopId, expectedBalanceChange, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldUseBillShopAndProduct_NotExternalInput()
    {
        // Ensures balance is derived from bill.Shop.Commission and bill.Product.Price,
        // not from any other source that could be manipulated
        var billId = new BillId(1);
        var shopId = new ShopId(1);
        var productId = new ProductId(1);
        var shop = new Shop { Id = shopId, Commission = new Commission(15m) };
        var product = new Product { Id = productId, Price = 200m };
        var bill = new Bill
        {
            Id = billId,
            ShopId = shopId,
            ProductId = productId,
            Shop = shop,
            Product = product,
            Payment = new Payment()
        };
        var request = new CompletePaymentRequest(billId);
        _mockBillRepository
            .Setup(r => r.GetByIdNoTrackingAsync(billId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(bill);
        _mockPurchaseRepository
            .Setup(r => r.CreateAsync(It.IsAny<Purchase>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PurchaseId(1));

        await _handler.Handle(request, CancellationToken.None);

        // 200 * (1 - 0.15) = 170
        _mockShopRepository.Verify(
            r => r.UpdateBalanceAsync(bill.ShopId, 170m, It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
