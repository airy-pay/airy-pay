using AiryPay.Shared.Settings.AppSettingsNested.PaymentSystemsSettings;
using YamlDotNet.Serialization;

namespace AiryPay.Shared.Settings.AppSettingsNested;

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
    
    /// <summary>
    /// Minimal money withdrawal amount
    /// </summary>
    /// <exception cref="ArgumentException">
    /// Thrown is value if below zero
    /// </exception>
    [YamlMember(typeof(decimal), Alias = "minimalWithdrawalAmount")]
    public required decimal MinimalWithdrawalAmount { get; set; }

    [YamlMember(typeof(RuKassaSettings), Alias = "ruKassaSettings")]
    public required RuKassaSettings RuKassaSettings { get; set; }
    
    [YamlMember(typeof(FinPaySettings), Alias = "finPaySettings")]
    public required FinPaySettings FinPaySettings { get; set; }
    
    [YamlMember(typeof(StripeSettings), Alias = "stripeSettings")]
    public required StripeSettings StripeSettings { get; set; }
    
    [YamlMember(typeof(SquareSettings), Alias = "squareSettings")]
    public required SquareSettings SquareSettings { get; set; }
    
    [YamlMember(typeof(PayPalSettings), Alias = "payPalSettings")]
    public required PayPalSettings PayPalSettings { get; set; }
    
    [YamlMember(typeof(IList<PaymentMethod>), Alias = "paymentMethods")]
    public required IList<PaymentMethod> PaymentMethods { get; set; }
}