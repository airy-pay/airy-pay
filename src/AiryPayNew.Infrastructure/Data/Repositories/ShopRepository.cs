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

    public override async Task<Shop?> GetByIdAsync(
        ShopId id, CancellationToken cancellationToken)
    {
        return await _dbContext.Shops
            .Include(x => x.Products)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken: cancellationToken);
    }

    public override async Task<Shop?> GetByIdNoTrackingAsync(
        ShopId id, CancellationToken cancellationToken)
    {
        return await _dbContext.Shops
            .AsNoTracking()
            .Include(x => x.Products)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken: cancellationToken);
    }
    
    public async Task<IList<Purchase>> GetShopPurchasesAsync(
        ShopId id, int amount, CancellationToken cancellationToken)
    {
        return await _dbContext.Purchases
            .AsNoTracking()
            .Include(x => x.Product)
            .Include(x => x.Bill)
            .OrderByDescending(x => x.DateTime)
            .Take(amount)
            .ToListAsync(cancellationToken: cancellationToken); 
    }

    public async Task<IList<Withdrawal>> GetShopWithdrawalsAsync(
        ShopId id, int amount, CancellationToken cancellationToken)
    {
        return await _dbContext.Withdrawals
            .AsNoTracking()
            .OrderByDescending(x => x.DateTime)
            .Take(amount)
            .ToListAsync(cancellationToken: cancellationToken); 
    }

    public async Task BlockAsync(ShopId shopId, CancellationToken cancellationToken)
    {
        await ChangeBlockedStatus(shopId, true, cancellationToken);
    }

    public async Task UnblockAsync(ShopId shopId, CancellationToken cancellationToken)
    {
        await ChangeBlockedStatus(shopId, false, cancellationToken);
    }

    public async Task<OperationResult<ShopId>> UpdateBalanceAsync(
        ShopId shopId, decimal change, CancellationToken cancellationToken)
    {
        var shop = await GetByIdAsync(shopId, cancellationToken);
        if (shop is null)
            return new OperationResult<ShopId>(false, "Entity not found", shopId);
        if (shop.Balance + change < 0)
            return new OperationResult<ShopId>(false, "Balance can't go below zero", shopId);

        shop.Balance += change;
        await _dbContext.SaveChangesAsync(cancellationToken);
        return OperationResult<ShopId>.Success(shopId);
    }

    private async Task ChangeBlockedStatus(ShopId shopId, bool blocked, CancellationToken cancellationToken)
    {
        var shop = await GetByIdAsync(shopId, cancellationToken);
        if (shop is null)
            return;

        shop.Blocked = blocked;
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}