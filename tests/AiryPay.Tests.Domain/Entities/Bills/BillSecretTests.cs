using AiryPay.Domain.Entities.Bills.BillSecrets;
using Moq;

namespace AiryPay.Tests.Domain.Entities.Bills;

public class BillSecretTests
{
    [Fact]
    public void Constructor_WithString_ShouldUseStringAsKey()
    {
        const string key = "my-secret-key-123";

        var billSecret = new BillSecret(key);

        billSecret.Key.Should().Be(key);
    }

    [Fact]
    public void Constructor_WithGenerator_ShouldUseGeneratedKey()
    {
        const string generatedKey = "generated-secret";
        var mockGenerator = new Mock<IBillSecretGenerator>();
        mockGenerator.Setup(g => g.Generate()).Returns(generatedKey);

        var billSecret = new BillSecret(mockGenerator.Object);

        billSecret.Key.Should().Be(generatedKey);
        mockGenerator.Verify(g => g.Generate(), Times.Once);
    }

    [Fact]
    public void Constructor_WithGenerator_ShouldProduceNonEmptyKey()
    {
        var mockGenerator = new Mock<IBillSecretGenerator>();
        mockGenerator.Setup(g => g.Generate()).Returns("non-empty-key");

        var billSecret = new BillSecret(mockGenerator.Object);

        billSecret.Key.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void Constructor_WithDifferentGenerators_ShouldProduceDifferentKeys()
    {
        var mock1 = new Mock<IBillSecretGenerator>();
        mock1.Setup(g => g.Generate()).Returns("key-from-generator-1");
        var mock2 = new Mock<IBillSecretGenerator>();
        mock2.Setup(g => g.Generate()).Returns("key-from-generator-2");

        var secret1 = new BillSecret(mock1.Object);
        var secret2 = new BillSecret(mock2.Object);

        secret1.Key.Should().NotBe(secret2.Key);
    }
}
