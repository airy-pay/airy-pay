using AiryPay.Discord.Utils;
using AiryPay.Domain.Common;

namespace AiryPay.Discord.Localization;

public class Localizer(Language language)
{
    public string GetString(string localizationRow)
    {
        return LocalizationManager.GetLocalized(localizationRow, language);
    }
}