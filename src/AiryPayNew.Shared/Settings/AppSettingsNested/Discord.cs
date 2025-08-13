using AiryPayNew.Shared.Settings.Models;

namespace AiryPayNew.Shared.Settings.AppSettingsNested;

public class Discord
{
    public bool UseStagingServer { get; set; }
    public ulong StagingServerId { get; set; }
    public required Color EmbedMessageColor { get; set; }
    public IEnumerable<RateLimit> RateLimiters { get; set; } = new List<RateLimit>();
}