using AiryPay.Application.Requests.Shops;
using AiryPay.Domain.Common;
using AiryPay.Domain.Entities.Shops;
using AiryPay.Tests.Application.TestHelpers;
using FluentAssertions;
using Moq;

namespace AiryPay.Tests.Application.Requests.Shops;

public class UpdateShopLanguageRequestHandlerTests
{
    private readonly Mock<IShopRepository> _mockShopRepository;
    private readonly UpdateShopLanguageRequestHandler _handler;

    public UpdateShopLanguageRequestHandlerTests()
    {
        _mockShopRepository = new Mock<IShopRepository>();
        var appSettings = AppSettingsFactory.Create(botSupportedLanguages: [new Language("en"), new Language("ru")]);
        _handler = new UpdateShopLanguageRequestHandler(_mockShopRepository.Object, appSettings);
    }

    [Fact]
    public async Task Handle_WhenShopNotFound_ShouldReturnShopNotFoundError()
    {
        var request = new UpdateShopLanguageRequest(new ShopId(1), new Language("en"));
        _mockShopRepository
            .Setup(r => r.GetByIdAsync(It.IsAny<ShopId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Shop?)null);

        var result = await _handler.Handle(request, CancellationToken.None);

        result.Successful.Should().BeFalse();
        result.ErrorType.Should().Be(UpdateShopLanguageRequest.Error.ShopNotFound);
    }

    [Fact]
    public async Task Handle_WhenLanguageNotSupported_ShouldReturnLanguageNotSupportedError()
    {
        var shopId = new ShopId(1);
        var shop = new Shop { Id = shopId, Language = new Language("en") };
        var request = new UpdateShopLanguageRequest(shopId, new Language("fr"));
        _mockShopRepository
            .Setup(r => r.GetByIdAsync(shopId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(shop);

        var result = await _handler.Handle(request, CancellationToken.None);

        result.Successful.Should().BeFalse();
        result.ErrorType.Should().Be(UpdateShopLanguageRequest.Error.LanguageNotSupported);
    }

    [Fact]
    public async Task Handle_WhenShopExistsAndLanguageSupported_ShouldCallUpdateLanguageAndReturnSuccess()
    {
        var shopId = new ShopId(1);
        var shop = new Shop { Id = shopId, Language = new Language("en") };
        var newLanguage = new Language("ru");
        var request = new UpdateShopLanguageRequest(shopId, newLanguage);
        _mockShopRepository
            .Setup(r => r.GetByIdAsync(shopId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(shop);

        var result = await _handler.Handle(request, CancellationToken.None);

        result.Successful.Should().BeTrue();
        _mockShopRepository.Verify(
            r => r.UpdateLanguageAsync(shopId, newLanguage, It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
