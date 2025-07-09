using AiryPayNew.Application.Requests.Withdrawals;
using AiryPayNew.Domain.Common;
using AiryPayNew.Domain.Entities.Shops;
using AiryPayNew.Domain.Entities.Withdrawals;
using FluentAssertions;
using Moq;

namespace AiryPayNew.Tests.Application;

public class CreateWithdrawalRequestHandlerTests
{
    private readonly Mock<IShopRepository> _mockShopRepository;
    private readonly CreateWithdrawalRequestHandler _handler;

    public CreateWithdrawalRequestHandlerTests()
    {
        Mock<IWithdrawalRepository> mockWithdrawalRepository = new();
        _mockShopRepository = new Mock<IShopRepository>();
        _handler = new CreateWithdrawalRequestHandler(mockWithdrawalRepository.Object, _mockShopRepository.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnError_WhenAmountIsLessThanOrEqualToZero()
    {
        // Arrange
        var request = new CreateWithdrawalRequest(
            1, 0, "card", "account123");

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.Successful.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_ShouldReturnError_WhenWayOrReceivingAccountNumberIsNullOrEmpty()
    {
        // Arrange
        var request1 = new CreateWithdrawalRequest(
            1, 100, "", "account123");
        var request2 = new CreateWithdrawalRequest(
            1, 100, "card", "");

        // Act
        var result1 = await _handler.Handle(request1, CancellationToken.None);
        var result2 = await _handler.Handle(request2, CancellationToken.None);

        // Assert
        result1.Successful.Should().BeFalse();
        result2.Successful.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_ShouldReturnError_WhenWayIsInvalid()
    {
        // Arrange
        var request = new CreateWithdrawalRequest(
            1, 100, "invalid_way", "account123");

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.Successful.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_ShouldReturnError_WhenShopIsNotFound()
    {
        // Arrange
        var request = new CreateWithdrawalRequest(
            1, 100, "card", "account123");
        _mockShopRepository.Setup(
                repo =>
                    repo.GetByIdNoTrackingAsync(
                        It.IsAny<ShopId>(),
                        It.IsAny<CancellationToken>()))
            .ReturnsAsync((Shop?) null);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.Successful.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_ShouldReturnError_WhenAmountIsLessThanMinimalWithdrawalAmount()
    {
        // Arrange
        var request = new CreateWithdrawalRequest(1, 400, "card", "account123");
        var shop = new Shop { Id = new ShopId(1), Balance = 1000, Language = new Language("en") };
        _mockShopRepository.Setup(repo => repo.GetByIdNoTrackingAsync(It.IsAny<ShopId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(shop);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.Successful.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_ShouldReturnError_WhenShopBalanceIsInsufficient()
    {
        // Arrange
        var request = new CreateWithdrawalRequest(1, 1000, "card", "account123");
        var shop = new Shop { Id = new ShopId(1), Balance = 500, Language = new Language("en") };
        _mockShopRepository.Setup(repo => repo.GetByIdNoTrackingAsync(It.IsAny<ShopId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(shop);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.Successful.Should().BeFalse();
    }
}