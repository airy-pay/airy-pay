using AiryPayNew.Domain.Common;
using AiryPayNew.Domain.Entities.Bills;
using AiryPayNew.Domain.Entities.Shops;

namespace AiryPayNew.Domain.Entities.Purchases;

public class Purchase : IEntity<PurchaseId>
{
    public PurchaseId Id { get; set; }
    public BillId BillId { get; set; }
    public DateTime DateTime { get; set; }
    public ShopId ShopId { get; set; }
    
    public Bill? Bill { get; set; }
    public Shop? Shop { get; set; }
}