using AiryPayNew.Domain.Common;

namespace AiryPayNew.Domain.Entities.Products;

public interface IProductRepository : IRepository<ProductId, Product>
{
    public Task Update(ProductId productId, string newEmoji, string newName, decimal newPrice, ulong newDiscordRole);
    public Task Delete(ProductId productId);
}