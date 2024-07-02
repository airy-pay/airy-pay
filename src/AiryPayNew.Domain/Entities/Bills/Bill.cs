using AiryPayNew.Domain.Common;
using AiryPayNew.Domain.Entities.Bills.BillSecrets;
using AiryPayNew.Domain.Entities.Bills.BillSecrets.BillSecretGenerators;
using AiryPayNew.Domain.Entities.Products;

namespace AiryPayNew.Domain.Entities.Bills;

public class Bill : IEntity<BillId>
{
    public BillId Id { get; set; }
    public ProductId ProductId { get; set; }
    public BillSecret BillSecret { get; set; } = new BillSecret(new GuidBillSecretGenerator());
    public BillStatus BillStatus { get; set; }
    
    public Product? Product { get; set; }
}