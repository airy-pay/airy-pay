using AiryPay.Domain.Entities.Shops;

namespace AiryPay.Tests.Domain.ValueObjects;

public class CommissionTests
{
    [Theory]
    [InlineData(0)]
    [InlineData(50)]
    [InlineData(100)]
    [InlineData(10.5)]
    [InlineData(0.01)]
    [InlineData(99.99)]
    public void Constructor_WithValidValue_ShouldSucceed(decimal value)
    {
        var commission = new Commission(value);

        commission.Value.Should().Be(value);
    }

    [Theory]
    [InlineData(-0.01)]
    [InlineData(-1)]
    [InlineData(-100)]
    public void Constructor_WithNegativeValue_ShouldThrowArgumentException(decimal value)
    {
        var act = () => new Commission(value);

        act.Should().Throw<ArgumentException>()
            .WithMessage("Commission has invalid value: " + value);
    }

    [Theory]
    [InlineData(100.01)]
    [InlineData(101)]
    [InlineData(1000)]
    public void Constructor_WithValueOver100_ShouldThrowArgumentException(decimal value)
    {
        var act = () => new Commission(value);

        act.Should().Throw<ArgumentException>()
            .WithMessage("Commission has invalid value: " + value);
    }
}
