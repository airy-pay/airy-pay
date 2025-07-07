using AiryPayNew.Domain.Common;
using AiryPayNew.Presentation.Utils;

namespace AiryPayNew.Presentation.Localization;

public class Localizer
{
    private readonly Language _language;
    
    public Localizer(Language language)
    {
        _language = language;
    }

    public string GetString(string localizationRow)
    {
        return LocalizationManager.GetLocalized(localizationRow, _language);
    }

    public static string GetString(string localizationRow, Language language)
    {
        return LocalizationManager.GetLocalized(localizationRow, language);
    }
}