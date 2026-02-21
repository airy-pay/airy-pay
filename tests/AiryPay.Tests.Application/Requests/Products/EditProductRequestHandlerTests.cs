using AiryPay.Application.Requests.Products;
using AiryPay.Domain.Common;
using AiryPay.Domain.Entities.Products;
using AiryPay.Domain.Entities.Shops;
using FluentAssertions;
using FluentValidation;
using Moq;

namespace AiryPay.Tests.Application.Requests.Products;

public class EditProductRequestHandlerTests
{
    private readonly Mock<IShopRepository> _mockShopRepository;
    private readonly Mock<IProductRepository> _mockProductRepository;
    private readonly Mock<IValidator<ProductModel>> _mockValidator;
    private readonly EditProductRequestHandler _handler;

    public EditProductRequestHandlerTests()
    {
        _mockShopRepository = new Mock<IShopRepository>();
        _mockProductRepository = new Mock<IProductRepository>();
        _mockValidator = new Mock<IValidator<ProductModel>>();
        _handler = new EditProductRequestHandler(
            _mockShopRepository.Object,
            _mockProductRepository.Object,
            _mockValidator.Object);
    }

    [Fact]
    public async Task Handle_WhenValidationFails_ShouldReturnValidationFailedError()
    {
        var request = new EditProductRequest(
            new ShopId(1), new ProductId(1), new ProductModel("😀", "AB", 50, 0));
        _mockValidator
            .Setup(v => v.ValidateAsync(It.IsAny<ProductModel>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult(
                [new FluentValidation.Results.ValidationFailure("Name", "Too short")]));

        var result = await _handler.Handle(request, CancellationToken.None);

        result.Successful.Should().BeFalse();
        result.ErrorType.Should().Be(EditProductRequest.Error.ValidationFailed);
    }

    [Fact]
    public async Task Handle_WhenShopNotFound_ShouldReturnShopNotFoundError()
    {
        var request = new EditProductRequest(
            new ShopId(1), new ProductId(1), new ProductModel("😀", "Valid Name", 500, 0));
        _mockValidator
            .Setup(v => v.ValidateAsync(It.IsAny<ProductModel>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());
        _mockShopRepository
            .Setup(r => r.GetByIdNoTrackingAsync(It.IsAny<ShopId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Shop?)null);

        var result = await _handler.Handle(request, CancellationToken.None);

        result.Successful.Should().BeFalse();
        result.ErrorType.Should().Be(EditProductRequest.Error.ShopNotFound);
    }

    [Fact]
    public async Task Handle_WhenShopBlocked_ShouldReturnShopIsBlockedError()
    {
        var shopId = new ShopId(1);
        var shop = new Shop { Id = shopId, Blocked = true, Language = new Language("en") };
        var request = new EditProductRequest(shopId, new ProductId(1), new ProductModel("😀", "Valid Name", 500, 0));
        _mockValidator
            .Setup(v => v.ValidateAsync(It.IsAny<ProductModel>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());
        _mockShopRepository
            .Setup(r => r.GetByIdNoTrackingAsync(shopId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(shop);

        var result = await _handler.Handle(request, CancellationToken.None);

        result.Successful.Should().BeFalse();
        result.ErrorType.Should().Be(EditProductRequest.Error.ShopIsBlocked);
    }

    [Fact]
    public async Task Handle_WhenProductNotFound_ShouldReturnProductNotFoundError()
    {
        var shopId = new ShopId(1);
        var shop = new Shop { Id = shopId, Language = new Language("en") };
        var request = new EditProductRequest(shopId, new ProductId(1), new ProductModel("😀", "Valid Name", 500, 0));
        _mockValidator
            .Setup(v => v.ValidateAsync(It.IsAny<ProductModel>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());
        _mockShopRepository
            .Setup(r => r.GetByIdNoTrackingAsync(shopId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(shop);
        _mockProductRepository
            .Setup(r => r.GetByIdNoTrackingAsync(It.IsAny<ProductId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product?)null);

        var result = await _handler.Handle(request, CancellationToken.None);

        result.Successful.Should().BeFalse();
        result.ErrorType.Should().Be(EditProductRequest.Error.ProductNotFound);
    }

    [Fact]
    public async Task Handle_WhenProductShopIdDoesNotMatch_ShouldReturnInvalidShopIdError()
    {
        var shopId = new ShopId(1);
        var shop = new Shop { Id = shopId, Language = new Language("en") };
        var product = new Product { Id = new ProductId(1), ShopId = new ShopId(999) };
        var request = new EditProductRequest(shopId, new ProductId(1), new ProductModel("😀", "Valid Name", 500, 0));
        _mockValidator
            .Setup(v => v.ValidateAsync(It.IsAny<ProductModel>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());
        _mockShopRepository
            .Setup(r => r.GetByIdNoTrackingAsync(shopId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(shop);
        _mockProductRepository
            .Setup(r => r.GetByIdNoTrackingAsync(It.IsAny<ProductId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        var result = await _handler.Handle(request, CancellationToken.None);

        result.Successful.Should().BeFalse();
        result.ErrorType.Should().Be(EditProductRequest.Error.InvalidShopId);
    }

    [Fact]
    public async Task Handle_WhenValid_ShouldCallUpdateAsyncAndReturnSuccess()
    {
        var shopId = new ShopId(1);
        var productId = new ProductId(1);
        var shop = new Shop { Id = shopId, Language = new Language("en") };
        var product = new Product { Id = productId, ShopId = shopId };
        var productModel = new ProductModel("😀", "New Name", 600, 456);
        var request = new EditProductRequest(shopId, productId, productModel);
        _mockValidator
            .Setup(v => v.ValidateAsync(It.IsAny<ProductModel>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());
        _mockShopRepository
            .Setup(r => r.GetByIdNoTrackingAsync(shopId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(shop);
        _mockProductRepository
            .Setup(r => r.GetByIdNoTrackingAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        var result = await _handler.Handle(request, CancellationToken.None);

        result.Successful.Should().BeTrue();
        _mockProductRepository.Verify(
            r => r.UpdateAsync(
                productId,
                productModel.DiscordEmoji,
                productModel.Name,
                productModel.Price,
                productModel.DiscordRoleId,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
