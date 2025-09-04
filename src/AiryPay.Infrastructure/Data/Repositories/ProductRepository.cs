using AiryPay.Domain.Entities.Products;

namespace AiryPay.Infrastructure.Data.Repositories;

internal class ProductRepository(ApplicationDbContext dbContext) 
    : Repository<ProductId, Product>(dbContext), IProductRepository
{
    private readonly ApplicationDbContext _dbContext = dbContext;

    public Task UpdateAsync(
        ProductId productId,
        string newEmoji,
        string newName,
        decimal newPrice,
        ulong newDiscordRole,
        CancellationToken cancellationToken)
    {
        var updatedProduct = new Product
        {
            Id = productId,
            Emoji = newEmoji,
            Name = newName,
            Price = newPrice,
            DiscordRoleId = newDiscordRole
        };
        
        return UpdateAsync(updatedProduct, cancellationToken);
    }

    public async Task UpdateAsync(Product entity, CancellationToken cancellationToken)
    {
        var product = await GetByIdAsync(entity.Id, cancellationToken);
        if (product is null || entity.Price <= 0)
            return;
        
        product.Emoji = entity.Emoji;
        product.Name = entity.Name;
        product.Price = entity.Price;
        product.DiscordRoleId = entity.DiscordRoleId;

        await _dbContext.SaveChangesAsync(cancellationToken);
    }
    
    public async Task DeleteAsync(ProductId productId, CancellationToken cancellationToken)
    {
        var product = await GetByIdAsync(productId, cancellationToken);
        if (product is null)
            return;

        _dbContext.Remove(product);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}