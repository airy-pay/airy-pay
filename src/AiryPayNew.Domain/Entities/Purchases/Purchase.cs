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

    public Bill Bill { get; set; } = null!;
    public Product Product { get; set; } = null!;
    public Shop Shop { get; set; } = null!;
}