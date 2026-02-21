using AiryPay.Application.Payments;
using AiryPay.Application.Requests.Payments;
using AiryPay.Domain.Common;
using AiryPay.Domain.Common.Result;
using AiryPay.Domain.Entities.Bills;
using AiryPay.Domain.Entities.Products;
using AiryPay.Domain.Entities.Shops;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace AiryPay.Tests.Application.Requests.Payments;

public class CreatePaymentRequestHandlerTests
{
    private readonly Mock<IBillRepository> _mockBillRepository;
    private readonly Mock<IProductRepository> _mockProductRepository;
    private readonly Mock<IShopRepository> _mockShopRepository;
    private readonly Mock<ILogger<CreatePaymentRequestHandler>> _mockLogger;
    private CreatePaymentRequestHandler _handler;

    public CreatePaymentRequestHandlerTests()
    {
        _mockBillRepository = new Mock<IBillRepository>();
        _mockProductRepository = new Mock<IProductRepository>();
        _mockShopRepository = new Mock<IShopRepository>();
        _mockLogger = new Mock<ILogger<CreatePaymentRequestHandler>>();
    }

    [Fact]
    public async Task Handle_WhenShopNotFound_ShouldReturnShopNotFoundError()
    {
        var request = new CreatePaymentRequest(new ProductId(1), "FinPay", "method1", 100, 1);
        _mockShopRepository
            .Setup(r => r.GetByIdNoTrackingAsync(It.IsAny<ShopId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Shop?)null);
        _handler = CreateHandler(paymentServices: []);

        var result = await _handler.Handle(request, CancellationToken.None);

        result.Successful.Should().BeFalse();
        result.ErrorType.Should().Be(CreatePaymentRequest.Error.ShopNotFound);
    }

    [Fact]
    public async Task Handle_WhenShopBlocked_ShouldReturnShopIsBlockedError()
    {
        var shopId = new ShopId(1);
        var shop = new Shop { Id = shopId, Blocked = true, Language = new Language("en") };
        var request = new CreatePaymentRequest(new ProductId(1), "FinPay", "method1", 100, (ulong)shopId.Value);
        _mockShopRepository
            .Setup(r => r.GetByIdNoTrackingAsync(It.IsAny<ShopId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(shop);
        _handler = CreateHandler(paymentServices: []);

        var result = await _handler.Handle(request, CancellationToken.None);

        result.Successful.Should().BeFalse();
        result.ErrorType.Should().Be(CreatePaymentRequest.Error.ShopIsBlocked);
    }

    [Fact]
    public async Task Handle_WhenPaymentServiceNotFound_ShouldReturnPaymentServiceNotFoundError()
    {
        var shopId = new ShopId(1);
        var shop = new Shop { Id = shopId, Language = new Language("en") };
        var request = new CreatePaymentRequest(new ProductId(1), "NonExistentService", "method1", 100, (ulong)shopId.Value);
        _mockShopRepository
            .Setup(r => r.GetByIdNoTrackingAsync(It.IsAny<ShopId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(shop);
        var mockPaymentService = new Mock<IPaymentService>();
        mockPaymentService.Setup(s => s.GetServiceName()).Returns("FinPay");
        _handler = CreateHandler(paymentServices: [mockPaymentService.Object]);

        var result = await _handler.Handle(request, CancellationToken.None);

        result.Successful.Should().BeFalse();
        result.ErrorType.Should().Be(CreatePaymentRequest.Error.PaymentServiceNotFound);
    }

    [Fact]
    public async Task Handle_WhenProductNotFound_ShouldReturnProductNotFoundError()
    {
        var shopId = new ShopId(1);
        var shop = new Shop { Id = shopId, Language = new Language("en") };
        var request = new CreatePaymentRequest(new ProductId(1), "FinPay", "method1", 100, (ulong)shopId.Value);
        _mockShopRepository
            .Setup(r => r.GetByIdNoTrackingAsync(It.IsAny<ShopId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(shop);
        _mockProductRepository
            .Setup(r => r.GetByIdNoTrackingAsync(It.IsAny<ProductId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product?)null);
        var mockPaymentService = new Mock<IPaymentService>();
        mockPaymentService.Setup(s => s.GetServiceName()).Returns("FinPay");
        _handler = CreateHandler(paymentServices: [mockPaymentService.Object]);

        var result = await _handler.Handle(request, CancellationToken.None);

        result.Successful.Should().BeFalse();
        result.ErrorType.Should().Be(CreatePaymentRequest.Error.ProductNotFound);
    }

    [Fact]
    public async Task Handle_WhenProductShopIdDoesNotMatch_ShouldReturnAccessDeniedError()
    {
        var shopId = new ShopId(1);
        var shop = new Shop { Id = shopId, Language = new Language("en") };
        var product = new Product { Id = new ProductId(1), ShopId = new ShopId(999) };
        var request = new CreatePaymentRequest(new ProductId(1), "FinPay", "method1", 100, (ulong)shopId.Value);
        _mockShopRepository
            .Setup(r => r.GetByIdNoTrackingAsync(It.IsAny<ShopId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(shop);
        _mockProductRepository
            .Setup(r => r.GetByIdNoTrackingAsync(It.IsAny<ProductId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);
        var mockPaymentService = new Mock<IPaymentService>();
        mockPaymentService.Setup(s => s.GetServiceName()).Returns("FinPay");
        _handler = CreateHandler(paymentServices: [mockPaymentService.Object]);

        var result = await _handler.Handle(request, CancellationToken.None);

        result.Successful.Should().BeFalse();
        result.ErrorType.Should().Be(CreatePaymentRequest.Error.AccessDenied);
    }

    [Fact]
    public async Task Handle_WhenBillCreatedButGetByIdReturnsNull_ShouldReturnFailedToCreateError()
    {
        var shopId = new ShopId(1);
        var productId = new ProductId(1);
        var shop = new Shop { Id = shopId, Language = new Language("en") };
        var product = new Product { Id = productId, ShopId = shopId };
        var request = new CreatePaymentRequest(productId, "FinPay", "method1", 100, (ulong)shopId.Value);
        _mockShopRepository
            .Setup(r => r.GetByIdNoTrackingAsync(It.IsAny<ShopId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(shop);
        _mockProductRepository
            .Setup(r => r.GetByIdNoTrackingAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);
        _mockBillRepository
            .Setup(r => r.CreateAsync(It.IsAny<Bill>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new BillId(1));
        _mockBillRepository
            .Setup(r => r.GetByIdNoTrackingAsync(It.IsAny<BillId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Bill?)null);
        var mockPaymentService = new Mock<IPaymentService>();
        mockPaymentService.Setup(s => s.GetServiceName()).Returns("FinPay");
        _handler = CreateHandler(paymentServices: [mockPaymentService.Object]);

        var result = await _handler.Handle(request, CancellationToken.None);

        result.Successful.Should().BeFalse();
        result.ErrorType.Should().Be(CreatePaymentRequest.Error.FailedToCreate);
    }

    [Fact]
    public async Task Handle_WhenValid_ShouldReturnSuccessWithPaymentUrl()
    {
        var shopId = new ShopId(1);
        var productId = new ProductId(1);
        var billId = new BillId(1);
        var shop = new Shop { Id = shopId, Language = new Language("en") };
        var product = new Product { Id = productId, ShopId = shopId };
        var bill = new Bill
        {
            Id = billId,
            ShopId = shopId,
            ProductId = productId,
            Payment = null
        };
        const string paymentUrl = "https://payment.example/checkout";
        var request = new CreatePaymentRequest(productId, "FinPay", "method1", 100, (ulong)shopId.Value);
        _mockShopRepository
            .Setup(r => r.GetByIdNoTrackingAsync(It.IsAny<ShopId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(shop);
        _mockProductRepository
            .Setup(r => r.GetByIdNoTrackingAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);
        _mockBillRepository
            .Setup(r => r.CreateAsync(It.IsAny<Bill>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(billId);
        _mockBillRepository
            .Setup(r => r.GetByIdNoTrackingAsync(billId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(bill);
        var mockPaymentService = new Mock<IPaymentService>();
        mockPaymentService.Setup(s => s.GetServiceName()).Returns("FinPay");
        mockPaymentService
            .Setup(s => s.CreateAsync(It.IsAny<Bill>(), It.IsAny<string>()))
            .ReturnsAsync(Result<string, IPaymentService.Error>.Success(paymentUrl));
        _handler = CreateHandler(paymentServices: [mockPaymentService.Object]);

        var result = await _handler.Handle(request, CancellationToken.None);

        result.Successful.Should().BeTrue();
        result.Entity.Should().Be(paymentUrl);
    }

    private CreatePaymentRequestHandler CreateHandler(IEnumerable<IPaymentService> paymentServices)
    {
        return new CreatePaymentRequestHandler(
            _mockBillRepository.Object,
            _mockProductRepository.Object,
            _mockShopRepository.Object,
            paymentServices,
            _mockLogger.Object);
    }
}
