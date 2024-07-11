using AiryPayNew.Application.Common;
using AiryPayNew.Application.Payments;
using AiryPayNew.Domain.Entities.Bills;
using AiryPayNew.Domain.Entities.Products;
using AiryPayNew.Domain.Entities.Purchases;
using AiryPayNew.Domain.Entities.Shops;
using AiryPayNew.Domain.Entities.Withdrawals;
using AiryPayNew.Infrastructure.Data;
using AiryPayNew.Infrastructure.Data.Repositories;
using AiryPayNew.Infrastructure.Services;
using AiryPayNew.Infrastructure.Services.HealthChecks;
using AiryPayNew.Infrastructure.Services.Payments;
using AiryPayNew.Infrastructure.Utils;
using AiryPayNew.Shared.Settings;
using AiryPayNew.Shared.Settings.AppSettings;
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

        #endregion

        #region Add health checks

        serviceCollection.AddScoped<IDatabaseHealthCheckService, DatabaseHealthCheckService>();

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
        
        return serviceCollection;
    }
}