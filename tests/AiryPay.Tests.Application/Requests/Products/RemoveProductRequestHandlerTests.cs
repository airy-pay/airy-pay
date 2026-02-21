using AiryPay.Application.Requests.Products;
using AiryPay.Domain.Entities.Products;
using AiryPay.Domain.Entities.Shops;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace AiryPay.Tests.Application.Requests.Products;

public class RemoveProductRequestHandlerTests
{
    private readonly Mock<IProductRepository> _mockProductRepository;
    private readonly Mock<ILogger<RemoveProductRequestHandler>> _mockLogger;
    private readonly RemoveProductRequestHandler _handler;

    public RemoveProductRequestHandlerTests()
    {
        _mockProductRepository = new Mock<IProductRepository>();
        _mockLogger = new Mock<ILogger<RemoveProductRequestHandler>>();
        _handler = new RemoveProductRequestHandler(_mockProductRepository.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task Handle_WhenProductNotFound_ShouldNotCallDeleteAsync()
    {
        var request = new RemoveProductRequest(new ShopId(1), new ProductId(42));
        _mockProductRepository
            .Setup(r => r.GetByIdNoTrackingAsync(It.IsAny<ProductId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product?)null);

        await _handler.Handle(request, CancellationToken.None);

        _mockProductRepository.Verify(
            r => r.DeleteAsync(It.IsAny<ProductId>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_WhenProductShopIdDoesNotMatch_ShouldNotCallDeleteAsync()
    {
        var product = new Product { Id = new ProductId(42), ShopId = new ShopId(999) };
        var request = new RemoveProductRequest(new ShopId(1), new ProductId(42));
        _mockProductRepository
            .Setup(r => r.GetByIdNoTrackingAsync(It.IsAny<ProductId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        await _handler.Handle(request, CancellationToken.None);

        _mockProductRepository.Verify(
            r => r.DeleteAsync(It.IsAny<ProductId>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_WhenProductExistsAndShopMatches_ShouldCallDeleteAsync()
    {
        var shopId = new ShopId(1);
        var productId = new ProductId(42);
        var product = new Product { Id = productId, ShopId = shopId };
        var request = new RemoveProductRequest(shopId, productId);
        _mockProductRepository
            .Setup(r => r.GetByIdNoTrackingAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        await _handler.Handle(request, CancellationToken.None);

        _mockProductRepository.Verify(
            r => r.DeleteAsync(productId, It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
