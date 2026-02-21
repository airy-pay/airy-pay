using AiryPay.Domain.Entities.ShopComplaints;

namespace AiryPay.Tests.Domain.Entities.ShopComplaints;

public class ShopComplaintTests
{
    [Fact]
    public void Constants_ReasonMaxLength_ShouldBe64()
    {
        ShopComplaint.Constants.ReasonMaxLength.Should().Be(64);
    }

    [Fact]
    public void Constants_DetailsMaxLength_ShouldBe1024()
    {
        ShopComplaint.Constants.DetailsMaxLength.Should().Be(1024);
    }
}
