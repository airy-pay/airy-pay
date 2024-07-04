using AiryPayNew.Domain.Common;

namespace AiryPayNew.Domain.Entities.Shops;

public interface IShopRepository : IRepository<ShopId, Shop>
{
    public Task Block(ShopId shopId);
    public Task Unblock(ShopId shopId);
    public Task<OperationResult<ShopId>> UpdateBalance(ShopId shopId, decimal change);
}