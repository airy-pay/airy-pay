using System.Reflection;
using AiryPayNew.Web.Http.Middlewares;

namespace AiryPayNew.Web.Http;

public static class WebApplicationExtensions
{
    public static void AddHttpEndpoints(this WebApplication app)
    {
        app.UseMiddleware<IpWhitelistMiddleware>();
        app.AddEndpointsFromAssembly(Assembly.GetExecutingAssembly());
    }
}