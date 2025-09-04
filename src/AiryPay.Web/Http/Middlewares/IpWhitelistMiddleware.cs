using System.Net;
using AiryPay.Shared.Settings;

namespace AiryPay.Web.Http.Middlewares;

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
        if (_allowedIPs.Contains("*"))
        {
            await _next(context);
            return;
        }
        
        var remoteIp = context.Connection.RemoteIpAddress;

        string remoteIpString = remoteIp?.ToString() ?? string.Empty;

        if (remoteIp is { IsIPv4MappedToIPv6: true })
        {
            remoteIpString = remoteIp.MapToIPv4().ToString();
        }

        if (string.IsNullOrEmpty(remoteIpString) || !_allowedIPs.Contains(remoteIpString))
        {
            context.Response.StatusCode = (int) HttpStatusCode.Forbidden;
            await context.Response.WriteAsync("Forbidden: IP is not allowed.");
            return;
        }

        await _next(context);
    }
}
