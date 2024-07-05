namespace AiryPayNew.Discord.Settings;

public class Discord
{
    public bool UseStagingServer { get; set; }
    public ulong StagingServerId { get; set; }
    public IEnumerable<RateLimit> RateLimiters { get; set; } = new List<RateLimit>();
}