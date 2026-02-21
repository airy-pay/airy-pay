using AiryPay.Application.Requests.Shops;
using AiryPay.Domain.Common;
using AiryPay.Domain.Entities.Shops;
using AiryPay.Tests.Application.TestHelpers;
using FluentAssertions;
using Moq;

namespace AiryPay.Tests.Application.Requests.Shops;

public class CreateShopRequestHandlerTests
{
    private readonly Mock<IShopRepository> _mockShopRepository;
    private readonly CreateShopRequestHandler _handler;

    public CreateShopRequestHandlerTests()
    {
        _mockShopRepository = new Mock<IShopRepository>();
        var appSettings = AppSettingsFactory.Create(defaultShopCommission: 5m);
        _handler = new CreateShopRequestHandler(_mockShopRepository.Object, appSettings);
    }

    [Fact]
    public async Task Handle_WhenShopAlreadyExists_ShouldNotCallCreateAsync()
    {
        var shopId = new ShopId(1);
        var existingShop = new Shop { Id = shopId, Language = new Language("en") };
        var request = new CreateShopRequest(shopId);
        _mockShopRepository
            .Setup(r => r.GetByIdNoTrackingAsync(shopId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingShop);

        await _handler.Handle(request, CancellationToken.None);

        _mockShopRepository.Verify(
            r => r.CreateAsync(It.IsAny<Shop>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_WhenShopDoesNotExist_ShouldCreateShopWithCorrectDefaults()
    {
        var shopId = new ShopId(1);
        var request = new CreateShopRequest(shopId);
        _mockShopRepository
            .Setup(r => r.GetByIdNoTrackingAsync(shopId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Shop?)null);

        await _handler.Handle(request, CancellationToken.None);

        _mockShopRepository.Verify(
            r => r.CreateAsync(It.Is<Shop>(s =>
                s.Id == shopId &&
                s.Balance == 0 &&
                s.Blocked == false &&
                s.Commission.Value == 5m),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
