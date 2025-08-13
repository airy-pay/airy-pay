using YamlDotNet.Serialization;

namespace AiryPayNew.Shared.Settings.AppSettingsNested;

public class PayPal
{
    [YamlMember(typeof(int), Alias = "clientId")]
    public string ClientId { get; set; } = string.Empty;
    
    [YamlMember(typeof(int), Alias = "secret")]
    public string Secret { get; set; } = string.Empty;
    
    [YamlMember(typeof(int), Alias = "sandbox")]
    public bool Sandbox { get; set; } = true;
}