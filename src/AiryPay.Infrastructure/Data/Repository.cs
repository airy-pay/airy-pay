using AiryPay.Domain.Common;
using AiryPay.Domain.Common.Repositories;
using Microsoft.EntityFrameworkCore;

namespace AiryPay.Infrastructure.Data;

internal abstract class Repository<TId, TEntity>(ApplicationDbContext dbContext)
    : IDefaultRepository<TId, TEntity>
    where TId : IId
    where TEntity : class, IEntity<TId>
{
    public virtual async Task<TEntity?> GetByIdAsync(TId id, CancellationToken cancellationToken)
    {
        return await GetByIdAsync(id, true, cancellationToken);
    }
    
    public virtual async Task<TEntity?> GetByIdNoTrackingAsync(TId id, CancellationToken cancellationToken)
    {
        return await GetByIdAsync(id, false, cancellationToken);
    }
    
    public virtual async Task<TId> CreateAsync(TEntity data, CancellationToken cancellationToken)
    {
        dbContext.Set<TEntity>().Add(data);
        await dbContext.SaveChangesAsync(cancellationToken);
        return data.Id;
    }
    
    private async Task<TEntity?> GetByIdAsync(TId id, bool trackEntity, CancellationToken cancellationToken)
    {
        var query = dbContext.Set<TEntity>().AsQueryable();
        if (!trackEntity)
        {
            query = query.AsNoTracking();
        }

        return await query.FirstOrDefaultAsync(
            e => e.Id.Equals(id),
            cancellationToken: cancellationToken);
    }
}