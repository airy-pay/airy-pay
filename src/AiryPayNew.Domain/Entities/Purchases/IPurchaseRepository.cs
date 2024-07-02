using AiryPayNew.Domain.Common;

namespace AiryPayNew.Domain.Entities.Purchases;

public interface IPurchaseRepository : IRepository<PurchaseId, Purchase> { }