using System.Reflection;
using AiryPay.Web.Http.Middlewares;

namespace AiryPay.Web.Http;

public static class WebApplicationExtensions
{
    public static void AddHttpEndpoints(this WebApplication app)
    {
        app.UseMiddleware<IpWhitelistMiddleware>();
        app.AddEndpointsFromAssembly(Assembly.GetExecutingAssembly());
    }
}