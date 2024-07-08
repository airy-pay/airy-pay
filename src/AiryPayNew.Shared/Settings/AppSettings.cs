namespace AiryPayNew.Shared.Settings;

public class AppSettings
{
    public required string Language { get; set; }
    public required RuKassa RuKassa { get; set; }
    public required Discord Discord { get; set; }
}