using AiryPay.Application.Common;
using AiryPay.Application.Payments;
using AiryPay.Domain.Common;
using AiryPay.Infrastructure.Data;
using AiryPay.Infrastructure.Services.Messaging;
using AiryPay.Infrastructure.Services.Payment;
using AiryPay.Infrastructure.Utils;
using AiryPay.Shared.Messaging;
using AiryPay.Shared.Settings;
using FinPay.API;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Ru.Kassa;
using Scrutor;

namespace AiryPay.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection serviceCollection,
        AppSettings appSettings)
    {
        #region Add db context
        
        serviceCollection.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseNpgsql(ConnectionStringReader.GetString());
        });
        
        #endregion

        #region Add repositories

        serviceCollection.Scan(selector => selector
            .FromAssemblies(typeof(DependencyInjection).Assembly)
            .AddClasses(filter => filter
                .InNamespaces(nameof(Data.Repositories))
                .AssignableTo(typeof(IRepository))
            )
            .UsingRegistrationStrategy(RegistrationStrategy.Throw)
            .AsMatchingInterface()
            .WithScopedLifetime());
        
        #endregion

        #region Add payment services

        #region Add payment services dependencies
        
        serviceCollection.AddSingleton(new RuKassaClient(
            appSettings.PaymentSettings.RuKassaSettings.MerchantId,
            appSettings.PaymentSettings.RuKassaSettings.Token,
            appSettings.PaymentSettings.RuKassaSettings.UserEmail,
            appSettings.PaymentSettings.RuKassaSettings.UserPassword));
        serviceCollection.AddSingleton(new FinPayApiClient(
            appSettings.PaymentSettings.FinPaySettings.ShopId,
            appSettings.PaymentSettings.FinPaySettings.Key1,
            appSettings.PaymentSettings.FinPaySettings.Key2
        ));

        serviceCollection.AddHttpClient<IPaymentService, PayPalPaymentService>()
            .AddPaymentResilience();
        serviceCollection.AddHttpClient<IPaymentService, StripePaymentService>()
            .AddPaymentResilience();
        serviceCollection.AddHttpClient<IPaymentService, SquarePaymentService>()
            .AddPaymentResilience();
        
        #endregion

        serviceCollection.Scan(selector => selector
            .FromAssemblies(typeof(DependencyInjection).Assembly)
            .AddClasses(filter => filter
                .InNamespaces(nameof(Services.Payment))
                .AssignableTo(typeof(IPaymentService))
            )
            .UsingRegistrationStrategy(RegistrationStrategy.Throw)
            .AsMatchingInterface()
            .WithTransientLifetime());
        
        #endregion

        #region Add RabbitMQ messaging producers

        serviceCollection.AddSingleton<IRoleAssignmentQueueService, RabbitMqRoleAssignmentQueueService>();
        serviceCollection.AddHostedService(provider =>
            (RabbitMqRoleAssignmentQueueService) provider.GetRequiredService<IRoleAssignmentQueueService>());
        serviceCollection.AddSingleton<IRabbitMqConnectionManager, RabbitMqConnectionManager>();
        
        #endregion
        
        return serviceCollection;
    }
}