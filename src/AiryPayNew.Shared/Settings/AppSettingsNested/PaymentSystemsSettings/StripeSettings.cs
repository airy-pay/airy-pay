using YamlDotNet.Serialization;

namespace AiryPayNew.Shared.Settings.AppSettingsNested.PaymentSystemsSettings;

public class StripeSettings
{
    [YamlMember(typeof(RuKassaSettings), Alias = "apiKey")]
    public string ApiKey { get; set; } = string.Empty;
    
    [YamlMember(typeof(RuKassaSettings), Alias = "successUrl")]
    public string SuccessUrl { get; set; } = string.Empty;
    
    [YamlMember(typeof(RuKassaSettings), Alias = "cancelUrl")]
    public string CancelUrl { get; set; } = string.Empty;
}