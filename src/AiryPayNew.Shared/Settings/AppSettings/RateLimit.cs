namespace AiryPayNew.Shared.Settings.AppSettings;

public class RateLimit
{
    public int Limit { get; set; }
    public TimeSpan Period { get; set; }
    public TimeSpan BanPeriod { get; set; }
}