using Discord;

namespace AiryPayNew.Presentation.Utils;

public class ColorMapper
{
    public static Color Map(Shared.Settings.Models.Color appSettingsColor)
        => new(appSettingsColor.R, appSettingsColor.G, appSettingsColor.B);
}