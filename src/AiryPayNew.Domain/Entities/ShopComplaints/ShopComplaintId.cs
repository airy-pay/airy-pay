using AiryPayNew.Domain.Common;

namespace AiryPayNew.Domain.Entities.ShopComplaints;

public record struct ShopComplaintId(int Value) : IIdBase<int>
{
    public static IId Default() => new ShopComplaintId(1);
}