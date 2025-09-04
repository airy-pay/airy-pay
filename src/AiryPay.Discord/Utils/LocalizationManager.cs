using System.Globalization;
using System.Resources;
using AiryPay.Discord.Localization;
using AiryPay.Domain.Common;

namespace AiryPay.Discord.Utils;

public static class LocalizationManager
{
    private const string ResourceBaseName = "AiryPay.Discord.Localization.Localization";
    
    private static readonly ResourceManager ResourceManager = new (
        ResourceBaseName,
        typeof(Localizer).Assembly);

    public static string GetLocalized(string key, Language language)
    {
        var culture = new CultureInfo(language.Code);
        return ResourceManager.GetString(key, culture) ?? key;
    }
}