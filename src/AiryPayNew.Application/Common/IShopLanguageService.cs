using AiryPayNew.Domain.Common;

namespace AiryPayNew.Application.Common;

public interface IShopLanguageService
{
    public Task<OperationResult> UpdateLanguage(ulong shopId, Language language);
}