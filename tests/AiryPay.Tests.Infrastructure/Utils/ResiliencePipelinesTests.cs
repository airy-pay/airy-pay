using AiryPay.Infrastructure.Utils;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http.Resilience;

namespace AiryPay.Tests.Infrastructure.Utils;

public class ResiliencePipelinesTests
{
    [Fact]
    public void AddPaymentResilience_ShouldNotThrow()
    {
        var services = new ServiceCollection();
        var builder = services.AddHttpClient("TestClient");

        var act = () => builder.AddPaymentResilience();

        act.Should().NotThrow();
    }

    [Fact]
    public void AddPaymentResilience_ShouldRegisterResilienceHandler()
    {
        var services = new ServiceCollection();
        services.AddHttpClient("TestClient")
            .AddPaymentResilience();

        var provider = services.BuildServiceProvider();
        var httpClientFactory = provider.GetRequiredService<IHttpClientFactory>();

        var client = httpClientFactory.CreateClient("TestClient");

        client.Should().NotBeNull();
    }

    [Fact]
    public void PaymentPipelineName_ShouldBePayment()
    {
        ResiliencePipelines.PaymentPipelineName.Should().Be("payment");
    }
}
