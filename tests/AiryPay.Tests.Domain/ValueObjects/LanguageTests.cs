using AiryPay.Domain.Common;

namespace AiryPay.Tests.Domain.ValueObjects;

public class LanguageTests
{
    [Theory]
    [InlineData("en")]
    [InlineData("ru")]
    [InlineData("de")]
    [InlineData("EN")]
    public void Constructor_WithValidTwoLetterCode_ShouldSucceed(string code)
    {
        var language = new Language(code);

        language.Code.Should().Be(code);
    }

    [Theory]
    [InlineData("eng")]
    [InlineData("a")]
    [InlineData("")]
    public void Constructor_WithInvalidLength_ShouldThrowArgumentException(string code)
    {
        var act = () => new Language(code);

        act.Should().Throw<ArgumentException>()
            .WithMessage("Invalid language code");
    }

    [Fact]
    public void English_ShouldHaveCodeEn()
    {
        Language.English.Code.Should().Be("en");
    }

    [Theory]
    [InlineData("en", true)]
    [InlineData("ru", true)]
    [InlineData("eng", false)]
    [InlineData("a", false)]
    public void TryParse_ShouldReturnCorrectResult(string code, bool expectedSuccess)
    {
        var result = Language.TryParse(code, out var language);

        result.Should().Be(expectedSuccess);
        if (expectedSuccess)
            language.Code.Should().Be(code);
    }

    [Fact]
    public void TryParse_WhenInvalid_ShouldSetLanguageToEnglish()
    {
        Language.TryParse("invalid", out var language);

        language.Should().Be(Language.English);
        language.Code.Should().Be("en");
    }
}
