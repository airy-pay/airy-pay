using AiryPay.Discord.Utils;
using AiryPay.Shared.Settings.Models;

namespace AiryPay.Tests.Discord.Utils;

public class ColorMapperTests
{
    [Fact]
    public void Map_ShouldMapRgbToDiscordColor()
    {
        var appColor = new Color { R = 255, G = 128, B = 64 };

        var result = ColorMapper.Map(appColor);

        result.R.Should().Be(255);
        result.G.Should().Be(128);
        result.B.Should().Be(64);
    }

    [Fact]
    public void Map_WithZeroValues_ShouldReturnBlack()
    {
        var appColor = new Color { R = 0, G = 0, B = 0 };

        var result = ColorMapper.Map(appColor);

        result.R.Should().Be(0);
        result.G.Should().Be(0);
        result.B.Should().Be(0);
    }
}
