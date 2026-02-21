using AiryPay.Application.Requests.Shops;
using AiryPay.Domain.Entities.Shops;
using AiryPay.Domain.Entities.Withdrawals;
using FluentAssertions;
using Moq;

namespace AiryPay.Tests.Application.Requests.Shops;

public class GetShopWithdrawalsRequestHandlerTests
{
    private readonly Mock<IShopRepository> _mockShopRepository;
    private readonly GetShopWithdrawalsRequestHandler _handler;

    public GetShopWithdrawalsRequestHandlerTests()
    {
        _mockShopRepository = new Mock<IShopRepository>();
        _handler = new GetShopWithdrawalsRequestHandler(_mockShopRepository.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnResultFromRepository()
    {
        var shopId = new ShopId(1);
        var withdrawals = new List<Withdrawal>
        {
            new() { Id = new WithdrawalId(1), ShopId = shopId }
        };
        var request = new GetShopWithdrawalsRequest(shopId);
        _mockShopRepository
            .Setup(r => r.GetShopWithdrawalsAsync(shopId, 10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(withdrawals);

        var result = await _handler.Handle(request, CancellationToken.None);

        result.Should().BeSameAs(withdrawals);
    }
}
