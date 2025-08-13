namespace AiryPayNew.Shared.Settings.AppSettingsNested;

public class RateLimit
{
    public int Limit { get; set; }
    public TimeSpan Period { get; set; }
    public TimeSpan BanPeriod { get; set; }
}