using System.Globalization;

namespace AiryPayNew.Discord.Utils;

public static class LanguageChanger
{
    public static void Update(string language)
    {
        var newCulture = CultureInfo.CreateSpecificCulture(language);
        
        Thread.CurrentThread.CurrentUICulture = newCulture;
        Thread.CurrentThread.CurrentCulture = newCulture;
    }
}