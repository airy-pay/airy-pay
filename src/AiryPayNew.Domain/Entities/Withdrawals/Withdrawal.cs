using AiryPayNew.Domain.Common;
using AiryPayNew.Domain.Entities.Shops;

namespace AiryPayNew.Domain.Entities.Withdrawals;

public class Withdrawal : IEntity<WithdrawalId>
{
    public WithdrawalId Id { get; set; }
    public decimal Amount { get; set; }
    public WithdrawalStatus WithdrawalStatus { get; set; }
    public string Way { get; set; } = string.Empty;
    public DateTime DateTime { get; set; }
    public ShopId ShopId { get; set; }
    
    public Shop? Shop { get; set; }
}