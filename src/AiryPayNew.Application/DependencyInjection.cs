using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace AiryPayNew.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddMediatR(c =>
            c.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
        
        return serviceCollection;
    }
}