using YamlDotNet.Serialization;

namespace AiryPayNew.Shared.Settings.AppSettingsNested.PaymentSystemsSettings;

public class RuKassaSettings
{
    [YamlMember(typeof(RuKassaSettings), Alias = "merchantId")]
    public required int MerchantId { get; set; }
    
    [YamlMember(typeof(RuKassaSettings), Alias = "token")]
    public required string Token { get; set; }
    
    [YamlMember(typeof(RuKassaSettings), Alias = "userEmail")]
    public required string UserEmail { get; set; }
    
    [YamlMember(typeof(RuKassaSettings), Alias = "userPassword")]
    public required string UserPassword { get; set; }
}