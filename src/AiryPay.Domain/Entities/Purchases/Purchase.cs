using AiryPay.Domain.Common;
using AiryPay.Domain.Entities.Bills;
using AiryPay.Domain.Entities.Products;
using AiryPay.Domain.Entities.Shops;

namespace AiryPay.Domain.Entities.Purchases;

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