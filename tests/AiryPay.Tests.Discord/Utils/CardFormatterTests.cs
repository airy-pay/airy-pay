using AiryPay.Discord.Utils;

namespace AiryPay.Tests.Discord.Utils;

public class CardFormatterTests
{
    [Fact]
    public void Format_WithValidCardNumber_ShouldSplitIntoGroupsOfFour()
    {
        var result = CardFormatter.Format("1234567890123456");

        result.Should().Be("1234 5678 9012 3456");
    }

    [Theory]
    [InlineData("1234", "1234")]
    [InlineData("12345678", "1234 5678")]
    public void Format_WithShorterCardNumber_ShouldFormatCorrectly(string input, string expected)
    {
        var result = CardFormatter.Format(input);

        result.Should().Be(expected);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Format_WithNullOrEmpty_ShouldThrowArgumentException(string? input)
    {
        var act = () => CardFormatter.Format(input!);

        act.Should().Throw<ArgumentException>()
            .WithParameterName("creditCardNumber")
            .WithMessage("Credit card number cannot be null or empty*");
    }
}
