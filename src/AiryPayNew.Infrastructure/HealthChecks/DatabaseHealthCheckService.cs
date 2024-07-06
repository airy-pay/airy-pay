using AiryPayNew.Application.Common;
using AiryPayNew.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AiryPayNew.Infrastructure.HealthChecks;

internal class DatabaseHealthCheckService(ApplicationDbContext dbContext) : IDatabaseHealthCheckService
{
    public async Task CheckConnection()
    {
        await dbContext.Database.ExecuteSqlRawAsync("SELECT 1");
    }
}