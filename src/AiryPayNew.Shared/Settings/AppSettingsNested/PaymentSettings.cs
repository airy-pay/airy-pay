using AiryPayNew.Shared.Settings.AppSettingsNested.PaymentSystemsSettings;
using YamlDotNet.Serialization;

namespace AiryPayNew.Shared.Settings.AppSettingsNested;

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

    [YamlMember(typeof(RuKassaSettings), Alias = "ruKassa")]
    public required RuKassaSettings RuKassaSettings { get; set; }
    
    [YamlMember(typeof(FinPaySettings), Alias = "finPay")]
    public required FinPaySettings FinPaySettings { get; set; }
    
    [YamlMember(typeof(FinPaySettings), Alias = "stripe")]
    public required StripeSettings StripeSettings { get; set; }
    
    [YamlMember(typeof(IList<PaymentMethod>), Alias = "paymentMethods")]
    public required IList<PaymentMethod> PaymentMethods { get; set; }
}