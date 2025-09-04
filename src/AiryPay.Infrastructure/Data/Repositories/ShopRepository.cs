using AiryPay.Domain.Common;
using AiryPay.Domain.Common.Result;
using AiryPay.Domain.Entities.Purchases;
using AiryPay.Domain.Entities.ShopComplaints;
using AiryPay.Domain.Entities.Shops;
using AiryPay.Domain.Entities.Withdrawals;
using Microsoft.EntityFrameworkCore;

namespace AiryPay.Infrastructure.Data.Repositories;

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

    public async Task<IList<ShopComplaint>> GetShopComplaintsAsync(
        ShopId id, CancellationToken cancellationToken, ulong userId = 0)
    {
        var shop = await _dbContext.Shops
            .AsNoTracking()
            .Include(shop => shop.Complaints)
            .FirstOrDefaultAsync(
                shop => shop.Id == id,
                cancellationToken: cancellationToken);
        if (shop is null)
            return new List<ShopComplaint>();
        
        return shop.Complaints
            .Where(c =>
            {
                if (userId > 0)
                {
                    return c.CreatorDiscordUserId == userId;
                }

                return true;
            })
            .ToList(); 
    }

    public async Task BlockAsync(ShopId shopId, CancellationToken cancellationToken)
    {
        await ChangeBlockedStatus(shopId, true, cancellationToken);
    }

    public async Task UnblockAsync(ShopId shopId, CancellationToken cancellationToken)
    {
        await ChangeBlockedStatus(shopId, false, cancellationToken);
    }

    public async Task<Result> UpdateBalanceAsync(
        ShopId shopId, decimal change, CancellationToken cancellationToken)
    {
        var shop = await GetByIdAsync(shopId, cancellationToken);
        if (shop is null)
            return Result.Fail();
        if (shop.Balance + change < 0)
            return Result.Fail();

        shop.Balance += change;
        await _dbContext.SaveChangesAsync(cancellationToken);
        
        return Result.Success();
    }

    public async Task<Result> UpdateLanguageAsync(
        ShopId shopId, Language language, CancellationToken cancellationToken)
    {
        var shop = await GetByIdAsync(shopId, cancellationToken);
        if (shop is null)
            return Result.Fail();
        
        shop.Language = language;
        
        await _dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success();
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