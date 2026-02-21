using AiryPay.Domain.Entities.Bills.BillSecrets.BillSecretGenerators;

namespace AiryPay.Tests.Domain.Services;

public class GuidBillSecretGeneratorTests
{
    [Fact]
    public void Generate_ShouldReturnNonEmptyString()
    {
        var generator = new GuidBillSecretGenerator();

        var result = generator.Generate();

        result.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void Generate_ShouldReturnValidGuidFormat()
    {
        var generator = new GuidBillSecretGenerator();

        var result = generator.Generate();

        Guid.TryParse(result, out _).Should().BeTrue();
    }

    [Fact]
    public void Generate_MultipleCalls_ShouldReturnDifferentValues()
    {
        var generator = new GuidBillSecretGenerator();

        var result1 = generator.Generate();
        var result2 = generator.Generate();

        result1.Should().NotBe(result2);
    }
}
