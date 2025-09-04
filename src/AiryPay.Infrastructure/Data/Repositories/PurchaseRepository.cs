using AiryPay.Domain.Entities.Purchases;

namespace AiryPay.Infrastructure.Data.Repositories;

internal class PurchaseRepository(ApplicationDbContext dbContext)
    : Repository<PurchaseId, Purchase>(dbContext), IPurchaseRepository { }