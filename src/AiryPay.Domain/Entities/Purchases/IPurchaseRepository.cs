using AiryPay.Domain.Common.Repositories;

namespace AiryPay.Domain.Entities.Purchases;

public interface IPurchaseRepository : IDefaultRepository<PurchaseId, Purchase> { }