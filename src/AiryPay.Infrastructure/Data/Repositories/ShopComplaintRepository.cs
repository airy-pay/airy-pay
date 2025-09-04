using AiryPay.Domain.Entities.ShopComplaints;

namespace AiryPay.Infrastructure.Data.Repositories;

internal class ShopComplaintRepository(ApplicationDbContext dbContext)
    : Repository<ShopComplaintId, ShopComplaint>(dbContext), IShopComplaintRepository { }