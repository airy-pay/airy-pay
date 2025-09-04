using AiryPay.Application;
using AiryPay.Infrastructure;
using AiryPay.Shared.Settings;

namespace AiryPay.Web.Configuration;

public static class Services
{
    public static IServiceCollection AddServices(
        this IServiceCollection serviceCollection,
        AppSettings appSettings)
    {
        serviceCollection.AddApplication();
        serviceCollection.AddInfrastructure(appSettings);
        
        serviceCollection.AddSingleton(appSettings);
        
        return serviceCollection;
    }
}