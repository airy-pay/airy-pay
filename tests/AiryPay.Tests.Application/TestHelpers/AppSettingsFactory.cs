using AiryPay.Domain.Common;
using AiryPay.Shared.Settings;
using AiryPay.Shared.Settings.AppSettingsNested;
using AiryPay.Shared.Settings.AppSettingsNested.PaymentSystemsSettings;
using AiryPay.Shared.Settings.Models;

namespace AiryPay.Tests.Application.TestHelpers;

public static class AppSettingsFactory
{
    public static AppSettings Create(
        decimal defaultShopCommission = 10m,
        IList<Language>? botSupportedLanguages = null)
    {
        return new AppSettings
        {
            Language = "en",
            BotSupportedLanguages = botSupportedLanguages ??
            [
                new Language("en")
            ],
            Kestrel = new Kestrel(),
            Discord = new Discord
            {
                EmbedMessageColor = new Color
                {
                    R = 0,
                    G = 0,
                    B = 0
                }
            },
            PaymentSettings = new PaymentSettings
            {
                DefaultShopCommission = defaultShopCommission,
                MinimalWithdrawalAmount = 500,
                RuKassaSettings = new RuKassaSettings
                {
                    MerchantId = 0,
                    Token = "",
                    UserEmail = "",
                    UserPassword = ""
                },
                FinPaySettings = new FinPaySettings
                {
                    ShopId = 0,
                    Key1 = "",
                    Key2 = ""
                },
                StripeSettings = new StripeSettings(),
                SquareSettings = new SquareSettings(),
                PayPalSettings = new PayPalSettings(),
                PaymentMethods =
                [
                ]
            },
            Links = new BotLinks()
            {
                SupportUrl = "",
                TermsUrl = "",
            }
        };
    }
}
