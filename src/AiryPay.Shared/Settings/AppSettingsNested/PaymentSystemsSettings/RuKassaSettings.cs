using YamlDotNet.Serialization;

namespace AiryPay.Shared.Settings.AppSettingsNested.PaymentSystemsSettings;

public class RuKassaSettings
{
    [YamlMember(typeof(int), Alias = "merchantId")]
    public required int MerchantId { get; set; }
    
    [YamlMember(typeof(string), Alias = "token")]
    public required string Token { get; set; }
    
    [YamlMember(typeof(string), Alias = "userEmail")]
    public required string UserEmail { get; set; }
    
    [YamlMember(typeof(string), Alias = "userPassword")]
    public required string UserPassword { get; set; }
}