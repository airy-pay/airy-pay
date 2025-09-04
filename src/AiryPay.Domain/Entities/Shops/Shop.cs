using AiryPay.Domain.Common;
using AiryPay.Domain.Entities.Bills;
using AiryPay.Domain.Entities.Products;
using AiryPay.Domain.Entities.Purchases;
using AiryPay.Domain.Entities.ShopComplaints;
using AiryPay.Domain.Entities.Withdrawals;

namespace AiryPay.Domain.Entities.Shops;

public class Shop : IEntity<ShopId>
{
    public ShopId Id { get; set; }
    public decimal Balance { get; set; }
    public bool Blocked { get; set; } = false;
    public Commission Commission { get; set; } = new(10.0m);
    public Language Language { get; set; } = Language.English;
    
    public IList<Bill> Bills { get; set; } = new List<Bill>();
    public IList<Product> Products { get; set; } = new List<Product>();
    public IList<Purchase> Purchases { get; set; } = new List<Purchase>();
    public IList<Withdrawal> Withdrawals { get; set; } = new List<Withdrawal>();
    public IList<ShopComplaint> Complaints { get; set; } = new List<ShopComplaint>();
}