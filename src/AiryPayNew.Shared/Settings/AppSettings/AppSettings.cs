namespace AiryPayNew.Shared.Settings.AppSettings;

public class AppSettings
{
    public required string Language { get; set; }
    public required Discord Discord { get; set; }
    public required PaymentSettings PaymentSettings { get; set; }
}