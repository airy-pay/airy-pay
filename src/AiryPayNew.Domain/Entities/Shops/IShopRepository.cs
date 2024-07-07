using AiryPayNew.Domain.Common;
using AiryPayNew.Domain.Entities.Purchases;
using AiryPayNew.Domain.Entities.Withdrawals;

namespace AiryPayNew.Domain.Entities.Shops;

public interface IShopRepository : IRepository<ShopId, Shop>
{
    public Task<IList<Purchase>> GetShopPurchases(ShopId shopId, int amount);
    public Task<IList<Withdrawal>> GetShopWithdrawals(ShopId shopId, int amount);
    public Task Block(ShopId shopId);
    public Task Unblock(ShopId shopId);
    public Task<OperationResult<ShopId>> UpdateBalance(ShopId shopId, decimal change);
}