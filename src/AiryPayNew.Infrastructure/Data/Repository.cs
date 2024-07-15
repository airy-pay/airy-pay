using AiryPayNew.Domain.Common;
using AiryPayNew.Domain.Common.Repositories;
using Microsoft.EntityFrameworkCore;

namespace AiryPayNew.Infrastructure.Data;

internal abstract class Repository<TId, TEntity>(ApplicationDbContext dbContext)
    : IDefaultRepository<TId, TEntity>
    where TId : IId
    where TEntity : class, IEntity<TId>
{
    public virtual async Task<TEntity?> GetByIdAsync(TId id)
    {
        return await GetByIdAsync(id, true);
    }
    
    public virtual async Task<TEntity?> GetByIdNoTrackingAsync(TId id)
    {
        return await GetByIdAsync(id, false);
    }
    
    public virtual async Task<TId> Create(TEntity data)
    {
        dbContext.Set<TEntity>().Add(data);
        await dbContext.SaveChangesAsync();
        return data.Id;
    }
    
    private async Task<TEntity?> GetByIdAsync(TId id, bool trackEntity)
    {
        var query = dbContext.Set<TEntity>().AsQueryable();
        if (!trackEntity)
        {
            query = query.AsNoTracking();
        }

        return await query.FirstOrDefaultAsync(e => e.Id.Equals(id));
    }
}