using Discord;

namespace AiryPayNew.Discord.Utils;

public class ColorMapper
{
    public static Color Map(Shared.Settings.Models.Color appSettingsColor)
        => new(appSettingsColor.R, appSettingsColor.G, appSettingsColor.B);
}