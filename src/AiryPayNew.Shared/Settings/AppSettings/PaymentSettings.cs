using YamlDotNet.Serialization;

namespace AiryPayNew.Shared.Settings.AppSettings;

public class PaymentSettings
{
    [YamlMember(typeof(RuKassa), Alias = "ruKassa")]
    public required RuKassa RuKassa { get; set; }
    
    [YamlMember(typeof(FinPay), Alias = "finPay")]
    public required FinPay FinPay { get; set; }
    
    [YamlMember(typeof(IList<PaymentMethod>), Alias = "paymentMethods")]
    public required IList<PaymentMethod> PaymentMethods { get; set; }
}