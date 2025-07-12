using AiryPayNew.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AiryPayNew.Infrastructure.Services.HealthChecks;

internal class DatabaseHealthCheckService(
    ApplicationDbContext dbContext,
    ILogger<DatabaseHealthCheckService> logger) : IHostedService
{
    public async Task CheckConnection()
    {
        await dbContext.Database.ExecuteSqlRawAsync("SELECT 1");
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Starting database health checks");
        await CheckConnection();
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}