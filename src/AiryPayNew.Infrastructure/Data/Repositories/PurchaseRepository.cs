using AiryPayNew.Domain.Entities.Purchases;

namespace AiryPayNew.Infrastructure.Data.Repositories;

public class PurchaseRepository(ApplicationDbContext dbContext)
    : Repository<PurchaseId, Purchase>(dbContext), IPurchaseRepository { }