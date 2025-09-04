using AiryPay.Domain.Common;
using AiryPay.Domain.Entities.Shops;

namespace AiryPay.Domain.Entities.ShopComplaints;

public class ShopComplaint : IEntity<ShopComplaintId>
{
    public static class Constants
    {
        public const int ReasonMaxLength = 64;
        public const int DetailsMaxLength = 1024;
    }
    
    public ShopComplaintId Id { get; set; }
    public ShopId ShopId { get; set; }
    public ulong CreatorDiscordUserId { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string Details { get; set; } = string.Empty;
    
    public Shop Shop { get; set; } = null!;
}