using YamlDotNet.Serialization;

namespace AiryPay.Shared.Settings.AppSettingsNested.PaymentSystemsSettings;

public class PayPalSettings
{
    [YamlMember(typeof(int), Alias = "clientId")]
    public string ClientId { get; set; } = string.Empty;
    
    [YamlMember(typeof(int), Alias = "secret")]
    public string Secret { get; set; } = string.Empty;
    
    [YamlMember(typeof(int), Alias = "sandbox")]
    public bool Sandbox { get; set; } = true;
}