using System.Reflection;
using AiryPayNew.Discord.Http.Middlewares;

namespace AiryPayNew.Discord.Http;

public static class WebApplicationExtensions
{
    public static void AddHttpEndpoints(this WebApplication app)
    {
        app.UseMiddleware<IpWhitelistMiddleware>();
        app.AddEndpointsFromAssembly(Assembly.GetExecutingAssembly());
    }
}