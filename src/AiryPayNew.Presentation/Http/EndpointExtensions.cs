using System.Reflection;

namespace AiryPayNew.Discord.Http;

public static class EndpointExtensions
{
    public static void AddEndpointsFromAssembly(this WebApplication app, Assembly assembly)
    {
        var endpointTypes = assembly.GetTypes()
            .Where(t => t.IsClass
                        && t.IsAbstract
                        && t.IsSealed
                        && t.GetCustomAttribute<EndpointAttribute>() is not null);

        foreach (var type in endpointTypes)
        {
            var method = type.GetMethod("AddEndpoint", 
                BindingFlags.Static | BindingFlags.Public);
            if (method is not null)
            {
                method.Invoke(null, [app]);
            }
        }
    }
}