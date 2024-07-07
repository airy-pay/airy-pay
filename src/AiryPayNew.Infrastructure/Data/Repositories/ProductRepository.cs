using AiryPayNew.Domain.Entities.Products;

namespace AiryPayNew.Infrastructure.Data.Repositories;

internal class ProductRepository(ApplicationDbContext dbContext) 
    : Repository<ProductId, Product>(dbContext), IProductRepository
{
    private readonly ApplicationDbContext _dbContext = dbContext;

    public async Task Update(
        ProductId productId, string newEmoji, string newName, decimal newPrice, ulong newDiscordRole)
    {
        var product = await GetByIdAsync(productId);
        if (product is null || newPrice <= 0)
            return;
        
        product.Emoji = newEmoji;
        product.Name = newName;
        product.Price = newPrice;
        product.DiscordRoleId = newDiscordRole;

        await _dbContext.SaveChangesAsync();
    }

    public async Task Delete(ProductId productId)
    {
        var product = await GetByIdAsync(productId);
        if (product is null)
            return;

        _dbContext.Remove(product);
        await _dbContext.SaveChangesAsync();
    }
}