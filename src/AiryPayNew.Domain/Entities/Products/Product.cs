using AiryPayNew.Domain.Common;
using AiryPayNew.Domain.Entities.Bills;
using AiryPayNew.Domain.Entities.Shops;

namespace AiryPayNew.Domain.Entities.Products;

public class Product : IEntity<ProductId>
{
    public ProductId Id { get; set; }
    public string Emoji { get; set; } = ":gem:";
    public string Name { get; set; } = "Product";
    public decimal Price { get; set; }
    
    public virtual ShopId ShopId { get; set; }
    public virtual Shop? Shop { get; set; }
    public IList<Bill> Bills { get; set; } = new List<Bill>();
}