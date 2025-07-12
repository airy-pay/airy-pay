using AiryPayNew.Application;
using AiryPayNew.Infrastructure;
using AiryPayNew.Shared.Settings.AppSettings;

namespace AiryPayNew.Web.Configuration;

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