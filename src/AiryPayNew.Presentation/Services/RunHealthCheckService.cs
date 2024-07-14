using AiryPayNew.Application.Common;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AiryPayNew.Discord.Services;

public class RunHealthCheckService(
    IServiceScopeFactory serviceScopeFactory,
    ILogger<RunHealthCheckService> logger) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Starting database health checks");
        using var scope = serviceScopeFactory.CreateScope();
        var databaseHealthCheckService = scope.ServiceProvider.GetRequiredService<IDatabaseHealthCheckService>();
        await databaseHealthCheckService.CheckConnection();
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
