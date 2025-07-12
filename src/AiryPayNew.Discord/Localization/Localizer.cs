using AiryPayNew.Discord.Utils;
using AiryPayNew.Domain.Common;

namespace AiryPayNew.Discord.Localization;

public class Localizer(Language language)
{
    public string GetString(string localizationRow)
    {
        return LocalizationManager.GetLocalized(localizationRow, language);
    }
}