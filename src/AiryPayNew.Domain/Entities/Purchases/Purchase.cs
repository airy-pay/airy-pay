using AiryPayNew.Domain.Common;
using AiryPayNew.Domain.Entities.Bills;
using AiryPayNew.Domain.Entities.Shops;

namespace AiryPayNew.Domain.Entities.Purchases;

public class Purchase : IEntity<PurchaseId>
{
    public PurchaseId Id { get; set; }
    public DateTime DateTime { get; set; }
    
    public Bill? Bill { get; set; }
    public virtual Shop? Shop { get; set; }
}