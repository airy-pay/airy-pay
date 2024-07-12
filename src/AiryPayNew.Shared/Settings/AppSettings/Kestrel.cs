namespace AiryPayNew.Shared.Settings.AppSettings;

public class Kestrel
{
    public IList<string> AllowedIPs { get; set; } = new List<string>();
}