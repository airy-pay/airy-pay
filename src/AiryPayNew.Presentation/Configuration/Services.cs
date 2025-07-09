using AiryPayNew.Application;
using AiryPayNew.Application.Common;
using AiryPayNew.Presentation.Services;
using AiryPayNew.Infrastructure;
using AiryPayNew.Shared.Settings.AppSettings;
using GenericRateLimiter.Configuration;
using Sqids;

namespace AiryPayNew.Presentation.Configuration;

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
        serviceCollection.AddSingleton<IShopLanguageService, ShopLanguageService>();
        serviceCollection.AddHostedService<RunHealthCheckService>();
        
        return serviceCollection;
    }
}