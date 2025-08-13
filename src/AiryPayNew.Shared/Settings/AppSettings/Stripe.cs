using YamlDotNet.Serialization;

namespace AiryPayNew.Shared.Settings.AppSettings;

public class Stripe
{
    [YamlMember(typeof(RuKassa), Alias = "apiKey")]
    public string ApiKey { get; set; } = string.Empty;
    
    [YamlMember(typeof(RuKassa), Alias = "successUrl")]
    public string SuccessUrl { get; set; } = string.Empty;
    
    [YamlMember(typeof(RuKassa), Alias = "cancelUrl")]
    public string CancelUrl { get; set; } = string.Empty;
}