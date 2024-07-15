using AiryPayNew.Domain.Common.Repositories;

namespace AiryPayNew.Domain.Entities.Purchases;

public interface IPurchaseRepository : IDefaultRepository<PurchaseId, Purchase> { }