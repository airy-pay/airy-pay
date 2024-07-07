using AiryPayNew.Domain.Common;
using AiryPayNew.Domain.Entities.Purchases;
using AiryPayNew.Domain.Entities.Shops;
using AiryPayNew.Domain.Entities.Withdrawals;
using Microsoft.EntityFrameworkCore;

namespace AiryPayNew.Infrastructure.Data.Repositories;

internal class ShopRepository(ApplicationDbContext dbContext)
    : Repository<ShopId, Shop>(dbContext), IShopRepository
{
    private readonly ApplicationDbContext _dbContext = dbContext;

    public override async Task<Shop?> GetByIdAsync(ShopId id)
    {
        return await _dbContext.Shops
            .Include(x => x.Products)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<IList<Purchase>> GetShopPurchases(ShopId id, int amount)
    {
        var shop = await _dbContext.Shops
            .Include(x => x.Purchases)
            .FirstOrDefaultAsync(x => x.Id == id);

        return shop is not null ? shop.Purchases : new List<Purchase>();
    }

    public async Task<IList<Withdrawal>> GetShopWithdrawals(ShopId id, int amount)
    {
        var shop = await _dbContext.Shops
            .Include(x => x.Withdrawals)
            .FirstOrDefaultAsync(x => x.Id == id);

        return shop is not null ? shop.Withdrawals : new List<Withdrawal>();
    }

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