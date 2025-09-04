using AiryPay.Domain.Common.Repositories;

namespace AiryPay.Domain.Entities.Products;

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