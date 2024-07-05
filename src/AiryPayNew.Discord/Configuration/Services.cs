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
        serviceCollection.AddInfrastructure();

        serviceCollection.AddRateLimiter<ulong>(options =>
        {
            options.AddRateLimiter(3, TimeSpan.FromSeconds(1));
            options.AddRateLimiter(200, TimeSpan.FromMinutes(10));
            options.AddRateLimiter(1200, TimeSpan.FromHours(2));
        });

        serviceCollection.AddSingleton(AppSettingsReader.GetSettings(configuration));
        
        return serviceCollection;
    }
}