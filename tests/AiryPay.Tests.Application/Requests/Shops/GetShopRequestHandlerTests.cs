using AiryPay.Application.Requests.Shops;
using AiryPay.Domain.Common;
using AiryPay.Domain.Entities.Shops;
using FluentAssertions;
using Moq;

namespace AiryPay.Tests.Application.Requests.Shops;

public class GetShopRequestHandlerTests
{
    private readonly Mock<IShopRepository> _mockShopRepository;
    private readonly GetShopRequestHandler _handler;

    public GetShopRequestHandlerTests()
    {
        _mockShopRepository = new Mock<IShopRepository>();
        _handler = new GetShopRequestHandler(_mockShopRepository.Object);
    }

    [Fact]
    public async Task Handle_WhenShopNotFound_ShouldReturnShopNotFoundError()
    {
        var request = new GetShopRequest(new ShopId(1));
        _mockShopRepository
            .Setup(r => r.GetByIdNoTrackingAsync(It.IsAny<ShopId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Shop?)null);

        var result = await _handler.Handle(request, CancellationToken.None);

        result.Successful.Should().BeFalse();
        result.Failed.Should().BeTrue();
        result.ErrorType.Should().Be(GetShopRequest.Error.ShopNotFound);
    }

    [Fact]
    public async Task Handle_WhenShopExists_ShouldReturnSuccessWithShop()
    {
        var shopId = new ShopId(1);
        var shop = new Shop { Id = shopId, Balance = 0, Language = new Language("en") };
        var request = new GetShopRequest(shopId);
        _mockShopRepository
            .Setup(r => r.GetByIdNoTrackingAsync(shopId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(shop);

        var result = await _handler.Handle(request, CancellationToken.None);

        result.Successful.Should().BeTrue();
        result.Entity.Should().Be(shop);
    }
}
