using AiryPayNew.Domain.Common.Repositories;

namespace AiryPayNew.Domain.Entities.Products;

public interface IProductRepository :
    IDefaultRepository<ProductId, Product>,
    IUpdateRepository<ProductId, Product>,
    IDeleteRepository<ProductId>,
    INoTrackRepository<ProductId, Product>
{
    public Task UpdateAsync(
        ProductId productId,
        string newEmoji,
        string newName,
        decimal newPrice,
        ulong newDiscordRole,
        CancellationToken cancellationToken);
}