using System.Net;
using AiryPayNew.Shared.Settings.AppSettings;

namespace AiryPayNew.Discord.Http.Middlewares;

public class IpWhitelistMiddleware
{
    private readonly HashSet<string> _allowedIPs;
    private readonly RequestDelegate _next;

    public IpWhitelistMiddleware(RequestDelegate next, AppSettings appSettings)
    {
        _next = next;
        _allowedIPs = new HashSet<string>(appSettings.Kestrel.AllowedIPs);
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var remoteIp = context.Connection.RemoteIpAddress;

        string remoteIpString = remoteIp?.ToString() ?? string.Empty;

        if (remoteIp is { IsIPv4MappedToIPv6: true })
        {
            remoteIpString = remoteIp.MapToIPv4().ToString();
        }

        if (string.IsNullOrEmpty(remoteIpString) || !_allowedIPs.Contains(remoteIpString))
        {
            context.Response.StatusCode = (int) HttpStatusCode.Forbidden;
            await context.Response.WriteAsync("Forbidden: Your IP is not allowed.");
            return;
        }

        await _next(context);
    }
}
