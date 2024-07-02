using AiryPayNew.Domain.Common;
using AiryPayNew.Domain.Entities.Bills.BillSecrets;
using AiryPayNew.Domain.Entities.Bills.BillSecrets.BillSecretGenerators;
using AiryPayNew.Domain.Entities.Products;
using AiryPayNew.Domain.Entities.Shops;

namespace AiryPayNew.Domain.Entities.Bills;

public class Bill : IEntity<BillId>
{
    public BillId Id { get; set; }
    public BillSecret BillSecret { get; set; } = new BillSecret(new GuidBillSecretGenerator());
    public BillStatus BillStatus { get; set; }
    
    public virtual Shop? Shop { get; set; }
    public virtual Product? Product { get; set; }
}