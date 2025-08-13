using AiryPayNew.Domain.Common;
using AiryPayNew.Shared.Settings.AppSettingsNested;

namespace AiryPayNew.Shared.Settings;

public class AppSettings
{
    public required string Language { get; set; }
    public required IList<Language> BotSupportedLanguages { get; set; }
    public required Kestrel Kestrel { get; set; }
    public required Discord Discord { get; set; }
    public required PaymentSettings PaymentSettings { get; set; }
}