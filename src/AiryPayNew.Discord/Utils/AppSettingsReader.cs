using AiryPayNew.Discord.Settings;
using Microsoft.Extensions.Configuration;

namespace AiryPayNew.Discord.Utils;

public static class AppSettingsReader
{
    public static AppSettings GetSettings(IConfiguration configuration)
    {
        var appSettings = configuration.Get<AppSettings>();
        if (appSettings is null)
            throw new InvalidDataException("Invalid data in app settings");

        return appSettings;
    }
}