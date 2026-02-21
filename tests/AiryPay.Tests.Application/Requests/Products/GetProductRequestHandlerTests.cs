using AiryPay.Application.Requests.Products;
using AiryPay.Domain.Entities.Products;
using AiryPay.Domain.Entities.Shops;
using FluentAssertions;
using Moq;

namespace AiryPay.Tests.Application.Requests.Products;

public class GetProductRequestHandlerTests
{
    private readonly Mock<IProductRepository> _mockProductRepository;
    private readonly GetProductRequestHandler _handler;

    public GetProductRequestHandlerTests()
    {
        _mockProductRepository = new Mock<IProductRepository>();
        _handler = new GetProductRequestHandler(_mockProductRepository.Object);
    }

    [Fact]
    public async Task Handle_WhenProductNotFound_ShouldReturnProductNotFoundError()
    {
        var request = new GetProductRequest(new ShopId(1), 42);
        _mockProductRepository
            .Setup(r => r.GetByIdNoTrackingAsync(It.IsAny<ProductId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product?)null);

        var result = await _handler.Handle(request, CancellationToken.None);

        result.Successful.Should().BeFalse();
        result.ErrorType.Should().Be(GetProductRequest.Error.ProductNotFound);
    }

    [Fact]
    public async Task Handle_WhenProductShopIdDoesNotMatchRequest_ShouldReturnUnauthorized()
    {
        var product = new Product { Id = new ProductId(42), ShopId = new ShopId(999) };
        var request = new GetProductRequest(new ShopId(1), 42);
        _mockProductRepository
            .Setup(r => r.GetByIdNoTrackingAsync(It.IsAny<ProductId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        var result = await _handler.Handle(request, CancellationToken.None);

        result.Successful.Should().BeFalse();
        result.ErrorType.Should().Be(GetProductRequest.Error.Unauthorized);
    }

    [Fact]
    public async Task Handle_WhenProductExistsAndShopMatches_ShouldReturnSuccessWithProduct()
    {
        var shopId = new ShopId(1);
        var product = new Product { Id = new ProductId(42), ShopId = shopId };
        var request = new GetProductRequest(shopId, 42);
        _mockProductRepository
            .Setup(r => r.GetByIdNoTrackingAsync(It.IsAny<ProductId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        var result = await _handler.Handle(request, CancellationToken.None);

        result.Successful.Should().BeTrue();
        result.Entity.Should().Be(product);
    }
}
