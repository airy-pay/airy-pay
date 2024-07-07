using AiryPayNew.Domain.Common;
using AiryPayNew.Domain.Entities.Bills;
using AiryPayNew.Domain.Entities.Products;
using AiryPayNew.Domain.Entities.Shops;

namespace AiryPayNew.Domain.Entities.Purchases;

public class Purchase : IEntity<PurchaseId>
{
    public PurchaseId Id { get; set; }
    public DateTime DateTime { get; set; }
    public ProductId ProductId { get; set; }
    public ShopId ShopId { get; set; }
    public BillId BillId { get; set; }
    
    public required Bill Bill { get; set; }
    public required Product Product { get; set; }
    public required Shop Shop { get; set; }
}