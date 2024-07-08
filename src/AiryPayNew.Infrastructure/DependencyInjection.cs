using AiryPayNew.Application.Common;
using AiryPayNew.Domain.Entities.Bills;
using AiryPayNew.Domain.Entities.Products;
using AiryPayNew.Domain.Entities.Purchases;
using AiryPayNew.Domain.Entities.Shops;
using AiryPayNew.Domain.Entities.Withdrawals;
using AiryPayNew.Infrastructure.Data;
using AiryPayNew.Infrastructure.Data.Repositories;
using AiryPayNew.Infrastructure.HealthChecks;
using AiryPayNew.Infrastructure.Services;
using AiryPayNew.Infrastructure.Utils;
using AiryPayNew.Shared.Settings;
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

        #region Add services

        serviceCollection.AddSingleton(new RuKassaClient(
            appSettings.RuKassa.MerchantId,
            appSettings.RuKassa.Token,
            appSettings.RuKassa.UserEmail,
            appSettings.RuKassa.UserPassword));
        serviceCollection.AddScoped<IPaymentService, PaymentService>();
        
        #endregion
        
        return serviceCollection;
    }
}