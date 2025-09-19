using AiryPay.Application.Common;
using AiryPay.Application.Payments;
using AiryPay.Domain.Entities.Bills;
using AiryPay.Domain.Entities.Products;
using AiryPay.Domain.Entities.Purchases;
using AiryPay.Domain.Entities.ShopComplaints;
using AiryPay.Domain.Entities.Shops;
using AiryPay.Domain.Entities.Withdrawals;
using AiryPay.Infrastructure.Data;
using AiryPay.Infrastructure.Data.Repositories;
using AiryPay.Infrastructure.Services.Messaging;
using AiryPay.Infrastructure.Services.Payment;
using AiryPay.Infrastructure.Utils;
using AiryPay.Shared.Messaging;
using AiryPay.Shared.Settings;
using FinPay.API;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Ru.Kassa;

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

        serviceCollection.AddScoped<IBillRepository, BillRepository>();
        serviceCollection.AddScoped<IProductRepository, ProductRepository>();
        serviceCollection.AddScoped<IPurchaseRepository, PurchaseRepository>();
        serviceCollection.AddScoped<IShopRepository, ShopRepository>();
        serviceCollection.AddScoped<IWithdrawalRepository, WithdrawalRepository>();
        serviceCollection.AddScoped<IShopComplaintRepository, ShopComplaintRepository>();

        #endregion

        #region Add payment services

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

        serviceCollection.AddHttpClient<IPaymentService, PayPalPaymentService>();
        serviceCollection.AddHttpClient<IPaymentService, StripePaymentService>();
        serviceCollection.AddHttpClient<IPaymentService, SquarePaymentService>();

        serviceCollection.AddTransient<IPaymentService, RuKassaPaymentService>();
        serviceCollection.AddTransient<IPaymentService, FinPayPaymentService>();
        serviceCollection.AddTransient<IPaymentService, PayPalPaymentService>();
        serviceCollection.AddTransient<IPaymentService, StripePaymentService>();
        serviceCollection.AddTransient<IPaymentService, SquarePaymentService>();
        
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