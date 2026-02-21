using AiryPay.Application.Requests.Payments;
using AiryPay.Application.Requests.Products;
using AiryPay.Domain.Common;
using AiryPay.Domain.Entities.Products;
using AiryPay.Domain.Entities.Shops;
using FluentAssertions;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Moq;

namespace AiryPay.Tests.Application.Requests.Products;

public class CreateProductRequestHandlerTests
{
    private readonly Mock<IProductRepository> _mockProductRepository;
    private readonly Mock<IShopRepository> _mockShopRepository;
    private readonly Mock<IValidator<ProductModel>> _mockValidator;
    private readonly Mock<ILogger<CreatePaymentRequestHandler>> _mockLogger;
    private readonly CreateProductRequestHandler _handler;

    public CreateProductRequestHandlerTests()
    {
        _mockProductRepository = new Mock<IProductRepository>();
        _mockShopRepository = new Mock<IShopRepository>();
        _mockValidator = new Mock<IValidator<ProductModel>>();
        _mockLogger = new Mock<ILogger<CreatePaymentRequestHandler>>();
        _handler = new CreateProductRequestHandler(
            _mockProductRepository.Object,
            _mockShopRepository.Object,
            _mockValidator.Object,
            _mockLogger.Object);
    }

    [Fact]
    public async Task Handle_WhenValidationFails_ShouldReturnValidationFailedError()
    {
        var request = new CreateProductRequest(new ShopId(1), new ProductModel("😀", "AB", 50, 0));
        _mockValidator
            .Setup(v => v.ValidateAsync(It.IsAny<ProductModel>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult(
                [new FluentValidation.Results.ValidationFailure("Name", "Too short")]));

        var result = await _handler.Handle(request, CancellationToken.None);

        result.Successful.Should().BeFalse();
        result.ErrorType.Should().Be(CreateProductRequest.Error.ValidationFailed);
    }

    [Fact]
    public async Task Handle_WhenShopNotFound_ShouldReturnShopNotFoundError()
    {
        var request = new CreateProductRequest(new ShopId(1), new ProductModel("😀", "Valid Name", 500, 0));
        _mockValidator
            .Setup(v => v.ValidateAsync(It.IsAny<ProductModel>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());
        _mockShopRepository
            .Setup(r => r.GetByIdNoTrackingAsync(It.IsAny<ShopId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Shop?)null);

        var result = await _handler.Handle(request, CancellationToken.None);

        result.Successful.Should().BeFalse();
        result.ErrorType.Should().Be(CreateProductRequest.Error.ShopNotFound);
    }

    [Fact]
    public async Task Handle_WhenTooManyProducts_ShouldReturnTooManyProductsCreatedError()
    {
        var shopId = new ShopId(1);
        var products = Enumerable.Range(0, 26).Select(_ => new Product()).ToList();
        var shop = new Shop { Id = shopId, Language = new Language("en"), Products = products };
        var request = new CreateProductRequest(shopId, new ProductModel("😀", "Valid Name", 500, 0));
        _mockValidator
            .Setup(v => v.ValidateAsync(It.IsAny<ProductModel>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());
        _mockShopRepository
            .Setup(r => r.GetByIdNoTrackingAsync(shopId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(shop);

        var result = await _handler.Handle(request, CancellationToken.None);

        result.Successful.Should().BeFalse();
        result.ErrorType.Should().Be(CreateProductRequest.Error.TooManyProductsCreated);
    }

    [Fact]
    public async Task Handle_WhenShopBlocked_ShouldReturnShopIsBlockedError()
    {
        var shopId = new ShopId(1);
        var shop = new Shop { Id = shopId, Blocked = true, Language = new Language("en") };
        var request = new CreateProductRequest(shopId, new ProductModel("😀", "Valid Name", 500, 0));
        _mockValidator
            .Setup(v => v.ValidateAsync(It.IsAny<ProductModel>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());
        _mockShopRepository
            .Setup(r => r.GetByIdNoTrackingAsync(shopId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(shop);

        var result = await _handler.Handle(request, CancellationToken.None);

        result.Successful.Should().BeFalse();
        result.ErrorType.Should().Be(CreateProductRequest.Error.ShopIsBlocked);
    }

    [Fact]
    public async Task Handle_WhenValid_ShouldCallCreateAsyncAndReturnSuccess()
    {
        var shopId = new ShopId(1);
        var shop = new Shop { Id = shopId, Language = new Language("en") };
        var productModel = new ProductModel("😀", "Valid Name", 500, 123);
        var request = new CreateProductRequest(shopId, productModel);
        _mockValidator
            .Setup(v => v.ValidateAsync(It.IsAny<ProductModel>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());
        _mockShopRepository
            .Setup(r => r.GetByIdNoTrackingAsync(shopId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(shop);
        _mockProductRepository
            .Setup(r => r.CreateAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ProductId(1));

        var result = await _handler.Handle(request, CancellationToken.None);

        result.Successful.Should().BeTrue();
        _mockProductRepository.Verify(
            r => r.CreateAsync(It.Is<Product>(p =>
                p.Emoji == productModel.DiscordEmoji &&
                p.Name == productModel.Name &&
                p.Price == productModel.Price &&
                p.DiscordRoleId == productModel.DiscordRoleId &&
                p.ShopId == shopId),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
