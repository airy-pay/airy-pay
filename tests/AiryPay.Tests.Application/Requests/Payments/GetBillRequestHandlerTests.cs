using AiryPay.Application.Requests.Payments;
using AiryPay.Domain.Entities.Bills;
using FluentAssertions;
using Moq;

namespace AiryPay.Tests.Application.Requests.Payments;

public class GetBillRequestHandlerTests
{
    private readonly Mock<IBillRepository> _mockBillRepository;
    private readonly GetBillRequestHandler _handler;

    public GetBillRequestHandlerTests()
    {
        _mockBillRepository = new Mock<IBillRepository>();
        _handler = new GetBillRequestHandler(_mockBillRepository.Object);
    }

    [Fact]
    public async Task Handle_WhenBillNotFound_ShouldReturnNull()
    {
        var request = new GetBillRequest(new BillId(1));
        _mockBillRepository
            .Setup(r => r.GetByIdNoTrackingAsync(It.IsAny<BillId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Bill?)null);

        var result = await _handler.Handle(request, CancellationToken.None);

        result.Should().BeNull();
    }

    [Fact]
    public async Task Handle_WhenBillExists_ShouldReturnBill()
    {
        var billId = new BillId(1);
        var bill = new Bill
        {
            Id = billId,
            BillStatus = BillStatus.Unpaid,
            Payment = null
        };
        var request = new GetBillRequest(billId);
        _mockBillRepository
            .Setup(r => r.GetByIdNoTrackingAsync(billId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(bill);

        var result = await _handler.Handle(request, CancellationToken.None);

        result.Should().Be(bill);
    }
}
