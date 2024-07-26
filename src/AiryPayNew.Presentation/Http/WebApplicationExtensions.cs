using System.Reflection;
using AiryPayNew.Presentation.Http.Middlewares;

namespace AiryPayNew.Presentation.Http;

public static class WebApplicationExtensions
{
    public static void AddHttpEndpoints(this WebApplication app)
    {
        app.UseMiddleware<IpWhitelistMiddleware>();
        app.AddEndpointsFromAssembly(Assembly.GetExecutingAssembly());
    }
}