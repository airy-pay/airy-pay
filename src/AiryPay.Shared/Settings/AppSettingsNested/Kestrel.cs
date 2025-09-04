namespace AiryPay.Shared.Settings.AppSettingsNested;

public class Kestrel
{
    public IList<string> AllowedIPs { get; set; } = new List<string>();
}