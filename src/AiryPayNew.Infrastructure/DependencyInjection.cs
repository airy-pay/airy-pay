using AiryPayNew.Application.Common;
using AiryPayNew.Application.Payments;
using AiryPayNew.Domain.Entities.Bills;
using AiryPayNew.Domain.Entities.Products;
using AiryPayNew.Domain.Entities.Purchases;
using AiryPayNew.Domain.Entities.ShopComplaints;
using AiryPayNew.Domain.Entities.Shops;
using AiryPayNew.Domain.Entities.Withdrawals;
using AiryPayNew.Infrastructure.Data;
using AiryPayNew.Infrastructure.Data.Repositories;
using AiryPayNew.Infrastructure.Services.Messaging;
using AiryPayNew.Infrastructure.Services.Payment;
using AiryPayNew.Infrastructure.Utils;
using AiryPayNew.Shared.Messaging;
using AiryPayNew.Shared.Settings;
using FinPay.API;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Ru.Kassa;

namespace AiryPayNew.Infrastructure;

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
            appSettings.PaymentSettings.RuKassa.MerchantId,
            appSettings.PaymentSettings.RuKassa.Token,
            appSettings.PaymentSettings.RuKassa.UserEmail,
            appSettings.PaymentSettings.RuKassa.UserPassword));
        serviceCollection.AddSingleton(new FinPayApiClient(
            appSettings.PaymentSettings.FinPay.ShopId,
            appSettings.PaymentSettings.FinPay.Key1,
            appSettings.PaymentSettings.FinPay.Key2
            ));

        serviceCollection.AddTransient<IPaymentService, RuKassaPaymentService>();
        serviceCollection.AddTransient<IPaymentService, FinPayPaymentService>();
        
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