using YamlDotNet.Serialization;

namespace AiryPay.Shared.Settings.AppSettingsNested.PaymentSystemsSettings;

public class SquareSettings
{
    [YamlMember(typeof(string), Alias = "accessToken")]
    public string AccessToken { get; set; } = string.Empty;
    
    [YamlMember(typeof(bool), Alias = "sandbox")]
    public bool UseSandbox { get; set; } = true;
}