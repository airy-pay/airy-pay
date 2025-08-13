using YamlDotNet.Serialization;

namespace AiryPayNew.Shared.Settings.AppSettings;

public class PaymentSettings
{
    /// <summary>
    /// Shop commission used by default
    /// </summary>
    /// <exception cref="ArgumentException">
    /// Thrown is value if below zero
    /// </exception>
    [YamlMember(typeof(decimal), Alias = "defaultShopCommission")]
    public required decimal DefaultShopCommission { get; set; }

    [YamlMember(typeof(RuKassa), Alias = "ruKassa")]
    public required RuKassa RuKassa { get; set; }
    
    [YamlMember(typeof(FinPay), Alias = "finPay")]
    public required FinPay FinPay { get; set; }
    
    [YamlMember(typeof(IList<PaymentMethod>), Alias = "paymentMethods")]
    public required IList<PaymentMethod> PaymentMethods { get; set; }
}