using AiryPay.Application;
using AiryPay.Discord.Services.Messaging;
using AiryPay.Infrastructure;
using AiryPay.Shared.Settings;
using GenericRateLimiter.Configuration;
using Sqids;

namespace AiryPay.Discord.Configuration;

public static class Services
{
    public static IServiceCollection AddServices(
        this IServiceCollection serviceCollection,
        AppSettings appSettings)
    {
        serviceCollection.AddApplication();
        serviceCollection.AddInfrastructure(appSettings);

        serviceCollection.AddRateLimiter<ulong>(options =>
        {
            foreach (var rateLimit in appSettings.Discord.RateLimiters)
            {
                options.AddRateLimiter(rateLimit.Limit, rateLimit.Period, rateLimit.BanPeriod);
            }
        });

        serviceCollection.AddSingleton(new SqidsEncoder<long>(new()
        {
            MinLength = 8
        }));
        
        serviceCollection.AddSingleton(appSettings);

        #region Configure messaging consumers

        serviceCollection.AddSingleton<RoleAssignmentConsumer>();
        serviceCollection.AddHostedService<RoleAssignmentListenerService>();

        #endregion
        
        return serviceCollection;
    }
}