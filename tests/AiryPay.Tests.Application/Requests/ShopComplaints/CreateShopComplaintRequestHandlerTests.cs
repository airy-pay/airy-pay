using AiryPay.Application.Requests.ShopComplaints;
using AiryPay.Domain.Entities.ShopComplaints;
using AiryPay.Domain.Entities.Shops;
using FluentAssertions;
using Moq;

namespace AiryPay.Tests.Application.Requests.ShopComplaints;

public class CreateShopComplaintRequestHandlerTests
{
    private readonly Mock<IShopComplaintRepository> _mockComplaintRepository;
    private readonly Mock<IShopRepository> _mockShopRepository;
    private readonly CreateShopComplaintRequestHandler _handler;

    public CreateShopComplaintRequestHandlerTests()
    {
        _mockComplaintRepository = new Mock<IShopComplaintRepository>();
        _mockShopRepository = new Mock<IShopRepository>();
        _handler = new CreateShopComplaintRequestHandler(
            _mockComplaintRepository.Object,
            _mockShopRepository.Object);
    }

    [Theory]
    [InlineData(null, "details")]
    [InlineData("", "details")]
    [InlineData("reason", null)]
    [InlineData("reason", "")]
    public async Task Handle_WhenReasonOrDetailsNullOrEmpty_ShouldReturnNoReasonAndDetailsError(
        string? reason, string? details)
    {
        var request = new CreateShopComplaintRequest(new ShopId(1), 123, reason!, details!);

        var result = await _handler.Handle(request, CancellationToken.None);

        result.Successful.Should().BeFalse();
        result.ErrorType.Should().Be(CreateShopComplaintRequest.Error.NoReasonAndDetails);
    }

    [Fact]
    public async Task Handle_WhenUserHasTooManyComplaints_ShouldReturnTooManyComplaintsError()
    {
        var shopId = new ShopId(1);
        var creatorId = 123ul;
        var request = new CreateShopComplaintRequest(shopId, creatorId, "reason", "details");
        var existingComplaints = Enumerable.Range(0, 10).Select(_ => new Domain.Entities.ShopComplaints.ShopComplaint()).ToList();
        _mockShopRepository
            .Setup(r => r.GetShopComplaintsAsync(shopId, It.IsAny<CancellationToken>(), creatorId))
            .ReturnsAsync(existingComplaints);

        var result = await _handler.Handle(request, CancellationToken.None);

        result.Successful.Should().BeFalse();
        result.ErrorType.Should().Be(CreateShopComplaintRequest.Error.TooManyComplaints);
    }

    [Fact]
    public async Task Handle_WhenValid_ShouldCallCreateAsyncAndReturnSuccess()
    {
        var shopId = new ShopId(1);
        var creatorId = 123ul;
        const string reason = "Bad service";
        const string details = "Details here";
        var request = new CreateShopComplaintRequest(shopId, creatorId, reason, details);
        _mockShopRepository
            .Setup(r => r.GetShopComplaintsAsync(shopId, It.IsAny<CancellationToken>(), creatorId))
            .ReturnsAsync(new List<Domain.Entities.ShopComplaints.ShopComplaint>());

        var result = await _handler.Handle(request, CancellationToken.None);

        result.Successful.Should().BeTrue();
        _mockComplaintRepository.Verify(
            r => r.CreateAsync(It.Is<Domain.Entities.ShopComplaints.ShopComplaint>(c =>
                c.ShopId == shopId &&
                c.CreatorDiscordUserId == creatorId &&
                c.Reason == reason &&
                c.Details == details),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
