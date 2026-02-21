using AiryPay.Application.Requests.Withdrawals;
using AiryPay.Domain.Common;
using AiryPay.Domain.Entities.Shops;
using AiryPay.Domain.Entities.Withdrawals;
using FluentAssertions;
using Moq;

namespace AiryPay.Tests.Application;

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
            new ShopId(1), 0, "card", "account123");

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
            new ShopId(1), 100, "", "account123");
        var request2 = new CreateWithdrawalRequest(
            new ShopId(1), 100, "card", "");

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
            new ShopId(1), 100, "invalid_way", "account123");

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
            new ShopId(1), 100, "card", "account123");
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
        var request = new CreateWithdrawalRequest(new ShopId(1), 400, "card", "account123");
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
        var request = new CreateWithdrawalRequest(new ShopId(1), 1000, "card", "account123");
        var shop = new Shop { Id = new ShopId(1), Balance = 500, Language = new Language("en") };
        _mockShopRepository.Setup(repo => repo.GetByIdNoTrackingAsync(It.IsAny<ShopId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(shop);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.Successful.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_WhenValid_ShouldUpdateBalanceAndCreateWithdrawal()
    {
        // Arrange
        var shopId = new ShopId(1);
        var request = new CreateWithdrawalRequest(shopId, 1000, "card", "account123");
        var shop = new Shop { Id = shopId, Balance = 5000, Language = new Language("en") };
        Mock<IWithdrawalRepository> mockWithdrawalRepository = new();
        _mockShopRepository.Setup(repo => repo.GetByIdNoTrackingAsync(shopId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(shop);

        var handler = new CreateWithdrawalRequestHandler(mockWithdrawalRepository.Object, _mockShopRepository.Object);

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        result.Successful.Should().BeTrue();
        _mockShopRepository.Verify(
            repo => repo.UpdateBalanceAsync(shopId, -1000, It.IsAny<CancellationToken>()),
            Times.Once);
        mockWithdrawalRepository.Verify(
            repo => repo.CreateAsync(It.Is<Withdrawal>(w =>
                w.Amount == 1000 &&
                w.Way == "card" &&
                w.ReceivingAccountNumber == "account123" &&
                w.ShopId == shopId),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}