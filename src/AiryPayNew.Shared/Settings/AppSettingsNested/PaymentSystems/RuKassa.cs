using YamlDotNet.Serialization;

namespace AiryPayNew.Shared.Settings.AppSettingsNested.PaymentSystems;

public class RuKassa
{
    [YamlMember(typeof(RuKassa), Alias = "merchantId")]
    public required int MerchantId { get; set; }
    
    [YamlMember(typeof(RuKassa), Alias = "token")]
    public required string Token { get; set; }
    
    [YamlMember(typeof(RuKassa), Alias = "userEmail")]
    public required string UserEmail { get; set; }
    
    [YamlMember(typeof(RuKassa), Alias = "userPassword")]
    public required string UserPassword { get; set; }
}