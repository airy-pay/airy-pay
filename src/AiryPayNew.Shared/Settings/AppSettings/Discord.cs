using Discord;

namespace AiryPayNew.Shared.Settings.AppSettings;

public class Discord
{
    public bool UseStagingServer { get; set; }
    public ulong StagingServerId { get; set; }
    public Color EmbedMessageColor { get; set; }
    public IEnumerable<RateLimit> RateLimiters { get; set; } = new List<RateLimit>();
}