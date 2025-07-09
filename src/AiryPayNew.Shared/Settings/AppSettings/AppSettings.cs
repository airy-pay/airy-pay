using AiryPayNew.Domain.Common;

namespace AiryPayNew.Shared.Settings.AppSettings;

public class AppSettings
{
    public required string Language { get; set; }
    public required IList<Language> BotSupportedLanguages { get; set; }
    public required Kestrel Kestrel { get; set; }
    public required Discord Discord { get; set; }
    public required PaymentSettings PaymentSettings { get; set; }
}