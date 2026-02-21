using AiryPay.Application.Requests.Shops;
using AiryPay.Domain.Entities.Purchases;
using AiryPay.Domain.Entities.Shops;
using FluentAssertions;
using Moq;

namespace AiryPay.Tests.Application.Requests.Shops;

public class GetShopPurchasesRequestHandlerTests
{
    private readonly Mock<IShopRepository> _mockShopRepository;
    private readonly GetShopPurchasesRequestHandler _handler;

    public GetShopPurchasesRequestHandlerTests()
    {
        _mockShopRepository = new Mock<IShopRepository>();
        _handler = new GetShopPurchasesRequestHandler(_mockShopRepository.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnResultFromRepository()
    {
        var shopId = new ShopId(1);
        var purchases = new List<Purchase>
        {
            new() { Id = new PurchaseId(1), ShopId = shopId }
        };
        var request = new GetShopPurchasesRequest(shopId);
        _mockShopRepository
            .Setup(r => r.GetShopPurchasesAsync(shopId, 20, It.IsAny<CancellationToken>()))
            .ReturnsAsync(purchases);

        var result = await _handler.Handle(request, CancellationToken.None);

        result.Should().BeSameAs(purchases);
    }
}
