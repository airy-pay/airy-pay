using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace AiryPay.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddMediatR(c =>
            c.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
        
        serviceCollection.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        
        return serviceCollection;
    }
}