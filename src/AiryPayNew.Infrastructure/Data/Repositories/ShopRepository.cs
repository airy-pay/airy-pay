using AiryPayNew.Domain.Common;
using AiryPayNew.Domain.Entities.Shops;

namespace AiryPayNew.Infrastructure.Data.Repositories;

public class ShopRepository(ApplicationDbContext dbContext)
    : Repository<ShopId, Shop>(dbContext), IShopRepository
{
    private readonly ApplicationDbContext _dbContext = dbContext;

    public async Task Block(ShopId shopId)
    {
        await ChangeBlockedStatus(shopId, true);
    }

    public async Task Unblock(ShopId shopId)
    {
        await ChangeBlockedStatus(shopId, false);
    }

    public async Task<OperationResult<ShopId>> UpdateBalance(ShopId shopId, decimal change)
    {
        var shop = await GetByIdAsync(shopId);
        if (shop is null)
            return new OperationResult<ShopId>(false, "Entity not found", shopId);
        if (shop.Balance + change < 0)
            return new OperationResult<ShopId>(false, "Balance can't go below zero", shopId);

        shop.Balance += change;
        await _dbContext.SaveChangesAsync();
        return OperationResult<ShopId>.Success(shopId);
    }

    private async Task ChangeBlockedStatus(ShopId shopId, bool blocked)
    {
        var shop = await GetByIdAsync(shopId);
        if (shop is null)
            return;

        shop.Blocked = blocked;
        await _dbContext.SaveChangesAsync();
    }
}