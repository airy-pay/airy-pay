using AiryPayNew.Domain.Entities.Purchases;

namespace AiryPayNew.Infrastructure.Data.Repositories;

internal class PurchaseRepository(ApplicationDbContext dbContext)
    : Repository<PurchaseId, Purchase>(dbContext), IPurchaseRepository { }