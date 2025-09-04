using AiryPay.Domain.Common;

namespace AiryPay.Domain.Entities.ShopComplaints;

public record struct ShopComplaintId(int Value) : IIdBase<int>
{
    public static IId Default() => new ShopComplaintId(1);
}