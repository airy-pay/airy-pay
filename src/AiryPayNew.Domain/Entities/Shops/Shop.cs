using AiryPayNew.Domain.Common;
using AiryPayNew.Domain.Entities.Bills;
using AiryPayNew.Domain.Entities.Products;
using AiryPayNew.Domain.Entities.Purchases;
using AiryPayNew.Domain.Entities.Withdrawals;

namespace AiryPayNew.Domain.Entities.Shops;

public class Shop : IEntity<ShopId>
{
    public ShopId Id { get; set; }
    public decimal Balance { get; set; }
    public bool Blocked { get; set; } = false;
    public Commission Commission { get; set; } = new(10.0m);
    public required Language Language { get; set; }
    
    public IList<Bill> Bills { get; set; } = new List<Bill>();
    public IList<Product> Products { get; set; } = new List<Product>();
    public IList<Purchase> Purchases { get; set; } = new List<Purchase>();
    public IList<Withdrawal> Withdrawals { get; set; } = new List<Withdrawal>();
}