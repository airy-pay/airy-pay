using AiryPayNew.Domain.Common;
using AiryPayNew.Domain.Entities.Bills.BillSecrets;
using AiryPayNew.Domain.Entities.Bills.BillSecrets.BillSecretGenerators;
using AiryPayNew.Domain.Entities.Products;
using AiryPayNew.Domain.Entities.Purchases;
using AiryPayNew.Domain.Entities.Shops;

namespace AiryPayNew.Domain.Entities.Bills;

public class Bill : IEntity<BillId>
{
    public BillId Id { get; set; }
    public long PaymentSystemId { get; set; }
    public string PaymentSystemName { get; set; } = string.Empty;
    public string PaymentMethodId { get; set; } = string.Empty;
    public BillSecret BillSecret { get; set; } = new(new GuidBillSecretGenerator());
    public BillStatus BillStatus { get; set; }
    public ulong BuyerDiscordId { get; set; }
    public ProductId ProductId { get; set; }
    public ShopId ShopId { get; set; }

    public virtual Shop Shop { get; set; } = null!;
    public required Product Product { get; set; }
    public virtual Purchase? Purchase { get; set; }
}