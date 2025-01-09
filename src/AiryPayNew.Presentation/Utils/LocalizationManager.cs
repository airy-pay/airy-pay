using System.Globalization;
using System.Resources;
using AiryPayNew.Domain.Common;

namespace AiryPayNew.Presentation.Utils;

public static class LocalizationManager
{
    private static readonly ResourceManager ResourceManager = new(
        typeof(LocalizationManager).Namespace + ".Localization", 
        typeof(LocalizationManager).Assembly);

    public static string GetLocalizedString(string key, Language language)
    {
        var culture = new CultureInfo(language.Code);
        return ResourceManager.GetString(key, culture) ?? key;
    }
}