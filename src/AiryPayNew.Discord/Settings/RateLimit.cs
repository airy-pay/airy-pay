namespace AiryPayNew.Discord.Settings;

public class RateLimit
{
    public int Limit { get; set; }
    public TimeSpan Period { get; set; }
}