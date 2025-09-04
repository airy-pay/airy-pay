using AiryPay.Domain.Common;
using AiryPay.Domain.Entities.Bills.BillSecrets;
using AiryPay.Domain.Entities.Bills.BillSecrets.BillSecretGenerators;
using AiryPay.Domain.Entities.Products;
using AiryPay.Domain.Entities.Purchases;
using AiryPay.Domain.Entities.Shops;

namespace AiryPay.Domain.Entities.Bills;

public class Bill : IEntity<BillId>
{
    public BillId Id { get; set; }
    public required Payment Payment { get; set; }
    public BillSecret BillSecret { get; set; } = new(new GuidBillSecretGenerator());
    public BillStatus BillStatus { get; set; }
    public ulong BuyerDiscordId { get; set; }
    public ProductId ProductId { get; set; }
    public ShopId ShopId { get; set; }

    public virtual Shop Shop { get; set; } = null!;
    public virtual Product Product { get; set; } = null!;
    public virtual Purchase? Purchase { get; set; }
}