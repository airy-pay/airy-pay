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