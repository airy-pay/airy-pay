using AiryPayNew.Shared.Settings;
using Microsoft.Extensions.Configuration;

namespace AiryPayNew.Discord.Utils;

public static class AppSettingsReader
{
    public static AppSettings GetAppSettings(this IConfiguration configuration)
    {
        var appSettings = configuration.Get<AppSettings>();
        if (appSettings is null)
            throw new InvalidDataException("Invalid data in app settings");

        return appSettings;
    }
}