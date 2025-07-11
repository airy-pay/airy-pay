using System.Globalization;
using System.Resources;
using AiryPayNew.Discord.Localization;
using AiryPayNew.Domain.Common;

namespace AiryPayNew.Discord.Utils;

public static class LocalizationManager
{
    private const string ResourceBaseName = "AiryPayNew.Discord.Localization.Localization";
    
    private static readonly ResourceManager ResourceManager = new (
        ResourceBaseName,
        typeof(Localizer).Assembly);

    public static string GetLocalized(string key, Language language)
    {
        var culture = new CultureInfo(language.Code);
        return ResourceManager.GetString(key, culture) ?? key;
    }
}