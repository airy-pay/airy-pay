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
        
        return serviceCollection;
    }
}