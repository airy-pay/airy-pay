using AiryPayNew.Domain.Entities.ShopComplaints;

namespace AiryPayNew.Infrastructure.Data.Repositories;

internal class ShopComplaintRepository(ApplicationDbContext dbContext)
    : Repository<ShopComplaintId, ShopComplaint>(dbContext), IShopComplaintRepository { }