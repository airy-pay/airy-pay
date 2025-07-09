using AiryPayNew.Domain.Common;
using AiryPayNew.Shared.Settings.AppSettings;
using Microsoft.Extensions.Configuration;

namespace AiryPayNew.Shared.Utils;

public static class ConfigurationReader
{
    public static AppSettings GetAppSettings(this IConfiguration configuration)
    {
        var appSettings = configuration.Get<AppSettings>();
        if (appSettings is null)
            throw new InvalidDataException("Invalid data in app settings");

        appSettings.PaymentSettings = GetPaymentSettings(configuration);

        var botSupportedLanguages = configuration.GetSection("BotSupportedLanguages").Get<string[]>();
        if (botSupportedLanguages is null || botSupportedLanguages.Length == 0)
            throw new InvalidDataException("No `BotSupportedLanguages` found in app settings");
        
        appSettings.BotSupportedLanguages = botSupportedLanguages
            .Select(l => new Language(l))
            .ToList();
        
        return appSettings;
    }
    
    public static PaymentSettings GetPaymentSettings(this IConfiguration configuration)
    {
        var paymentSettings = configuration.Get<PaymentSettings>();
        if (paymentSettings is null)
            throw new InvalidDataException("Invalid data in payment settings");

        return paymentSettings;
    }
}