using AiryPay.Domain.Common;
using AiryPay.Discord.Utils;

namespace AiryPay.Tests.Discord.Utils;

public class LocalizationManagerTests
{
    [Fact]
    public void GetLocalized_WithKnownKeyAndEnglish_ShouldReturnLocalizedString()
    {
        var result = LocalizationManager.GetLocalized("balance", new Language("en"));

        result.Should().NotBeNullOrEmpty();
        result.Should().NotBe("balance");
    }

    [Fact]
    public void GetLocalized_WithUnknownKey_ShouldReturnKeyAsFallback()
    {
        var result = LocalizationManager.GetLocalized("nonexistent_key_xyz_123", new Language("en"));

        result.Should().Be("nonexistent_key_xyz_123");
    }

    [Fact]
    public void GetLocalized_WithDifferentLanguage_ShouldReturnLocalizedString()
    {
        var resultEn = LocalizationManager.GetLocalized("shopId", new Language("en"));
        var resultRu = LocalizationManager.GetLocalized("shopId", new Language("ru"));

        resultEn.Should().NotBeNullOrEmpty();
        resultRu.Should().NotBeNullOrEmpty();
    }
}
