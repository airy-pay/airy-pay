using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http.Resilience;
using Polly;

namespace AiryPay.Infrastructure.Utils;

public static class ResiliencePipelines
{
    public const string PaymentPipelineName = "payment";

    public static void AddPaymentResilience(this IHttpClientBuilder builder)
    {
        builder.AddResilienceHandler(PaymentPipelineName, pipeline => pipeline
            .AddTimeout(TimeSpan.FromSeconds(10))
            .AddRetry(new HttpRetryStrategyOptions
            {
                MaxRetryAttempts = 3,
                BackoffType = DelayBackoffType.Exponential,
                UseJitter = true,
                Delay = TimeSpan.FromSeconds(500)
            })
            .AddCircuitBreaker(new HttpCircuitBreakerStrategyOptions
            {
                SamplingDuration = TimeSpan.FromSeconds(30),
                FailureRatio = 0.5,
                MinimumThroughput = 10,
                BreakDuration = TimeSpan.FromSeconds(15)
            })
        );
    }
}
