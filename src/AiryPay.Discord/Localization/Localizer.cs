using System.Globalization;
using AiryPay.Domain.Common;

namespace AiryPay.Discord.Localization;

public class Localizer : Localization
{
    public Localizer(Language language) : base(new CultureInfo(language.Code)) { }

    public string GetString(string key) => _resourceManager.GetString(key, _culture) ?? key;
}
