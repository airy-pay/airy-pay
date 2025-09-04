using YamlDotNet.Serialization;

namespace AiryPay.Shared.Settings.AppSettingsNested.PaymentSystemsSettings;

public class FinPaySettings
{
    [YamlMember(typeof(int), Alias = "shopId")]
    public required int ShopId { get; set; }
    
    [YamlMember(typeof(string), Alias = "key1")]
    public required string Key1 { get; set; }
    
    [YamlMember(typeof(string), Alias = "key2")]
    public required string Key2 { get; set; }
}