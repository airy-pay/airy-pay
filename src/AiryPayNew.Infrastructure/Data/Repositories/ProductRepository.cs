using AiryPayNew.Domain.Entities.Products;

namespace AiryPayNew.Infrastructure.Data.Repositories;

internal class ProductRepository(ApplicationDbContext dbContext) 
    : Repository<ProductId, Product>(dbContext), IProductRepository
{
    private readonly ApplicationDbContext _dbContext = dbContext;

    public Task Update(
        ProductId productId,
        string newEmoji,
        string newName,
        decimal newPrice,
        ulong newDiscordRole)
    {
        var updatedProduct = new Product
        {
            Id = productId,
            Emoji = newEmoji,
            Name = newName,
            Price = newPrice,
            DiscordRoleId = newDiscordRole
        };
        
        return Update(updatedProduct);
    }

    public async Task Update(Product entity)
    {
        var product = await GetByIdAsync(entity.Id);
        if (product is null || entity.Price <= 0)
            return;
        
        product.Emoji = entity.Emoji;
        product.Name = entity.Name;
        product.Price = entity.Price;
        product.DiscordRoleId = entity.DiscordRoleId;

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