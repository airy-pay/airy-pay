using AiryPayNew.Discord.Utils;
using AiryPayNew.Infrastructure;
using GenericRateLimiter.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AiryPayNew.Discord.Configuration;

public static class Services
{
    public static IServiceCollection AddServices(
        this IServiceCollection serviceCollection,
        IConfiguration configuration)
    {
        var appSettings = configuration.GetAppSettings();
        
        serviceCollection.AddInfrastructure();

        serviceCollection.AddRateLimiter<ulong>(options =>
        {
            foreach (var rateLimit in appSettings.Discord.RateLimiters)
            {
                options.AddRateLimiter(rateLimit.Limit, rateLimit.Period);
            }
        });

        serviceCollection.AddSingleton(configuration.GetAppSettings());
        
        return serviceCollection;
    }
}