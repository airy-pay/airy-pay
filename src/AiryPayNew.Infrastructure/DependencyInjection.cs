using AiryPayNew.Domain.Entities.Bills;
using AiryPayNew.Domain.Entities.Products;
using AiryPayNew.Domain.Entities.Purchases;
using AiryPayNew.Domain.Entities.Shops;
using AiryPayNew.Domain.Entities.Withdrawals;
using AiryPayNew.Infrastructure.Data;
using AiryPayNew.Infrastructure.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AiryPayNew.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection serviceCollection)
    {
        #region Add db context
        
        serviceCollection.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseNpgsql(ConnectionStringReader.GetString());
        });
        
        #endregion

        #region Add repositories

        serviceCollection.AddScoped<IBillRepository, IBillRepository>();
        serviceCollection.AddScoped<IProductRepository, IProductRepository>();
        serviceCollection.AddScoped<IPurchaseRepository, IPurchaseRepository>();
        serviceCollection.AddScoped<IShopRepository, IShopRepository>();
        serviceCollection.AddScoped<IWithdrawalRepository, IWithdrawalRepository>();

        #endregion
        
        return serviceCollection;
    }
}