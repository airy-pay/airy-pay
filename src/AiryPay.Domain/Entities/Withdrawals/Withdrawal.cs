using AiryPay.Domain.Common;
using AiryPay.Domain.Entities.Shops;

namespace AiryPay.Domain.Entities.Withdrawals;

public class Withdrawal : IEntity<WithdrawalId>
{
    public WithdrawalId Id { get; set; }
    public decimal Amount { get; set; }
    public WithdrawalStatus WithdrawalStatus { get; set; } = WithdrawalStatus.InProcess;
    public string Way { get; set; } = string.Empty;
    public string ReceivingAccountNumber { get; set; } = string.Empty;
    public DateTime DateTime { get; set; }
    public ShopId ShopId { get; set; }
    
    public virtual Shop? Shop { get; set; }
}