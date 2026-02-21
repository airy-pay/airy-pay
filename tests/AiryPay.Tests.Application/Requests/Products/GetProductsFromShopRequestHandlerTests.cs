using AiryPay.Application.Requests.Products;
using AiryPay.Domain.Common;
using AiryPay.Domain.Entities.Products;
using AiryPay.Domain.Entities.Shops;
using FluentAssertions;
using Moq;

namespace AiryPay.Tests.Application.Requests.Products;

public class GetProductsFromShopRequestHandlerTests
{
    private readonly Mock<IShopRepository> _mockShopRepository;
    private readonly GetProductsFromShopRequestHandler _handler;

    public GetProductsFromShopRequestHandlerTests()
    {
        _mockShopRepository = new Mock<IShopRepository>();
        _handler = new GetProductsFromShopRequestHandler(_mockShopRepository.Object);
    }

    [Fact]
    public async Task Handle_WhenShopNotFound_ShouldReturnShopNotFoundError()
    {
        var request = new GetProductsFromShopRequest(new ShopId(1));
        _mockShopRepository
            .Setup(r => r.GetByIdNoTrackingAsync(It.IsAny<ShopId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Shop?)null);

        var result = await _handler.Handle(request, CancellationToken.None);

        result.Successful.Should().BeFalse();
        result.ErrorType.Should().Be(GetProductsFromShopRequest.Error.ShopNotFound);
    }

    [Fact]
    public async Task Handle_WhenShopExists_ShouldReturnSuccessWithShopProducts()
    {
        var shopId = new ShopId(1);
        var products = new List<Product>
        {
            new() { Id = new ProductId(1), ShopId = shopId, Name = "A" }
        };
        var shop = new Shop { Id = shopId, Language = new Language("en"), Products = products };
        var request = new GetProductsFromShopRequest(shopId);
        _mockShopRepository
            .Setup(r => r.GetByIdNoTrackingAsync(shopId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(shop);

        var result = await _handler.Handle(request, CancellationToken.None);

        result.Successful.Should().BeTrue();
        result.Entity.Should().BeSameAs(products);
    }
}
