using AiryPayNew.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace AiryPayNew.Infrastructure.Data;

internal abstract class Repository<TId, TEntity>(ApplicationDbContext dbContext) : IRepository<TId, TEntity>
    where TId : IId
    where TEntity : class, IEntity<TId>
{
    public async Task<TEntity?> GetByIdAsync(TId id)
    {
        return await dbContext.Set<TEntity>().FirstOrDefaultAsync(e => e.Id.Equals(id));
    }

    public async Task<TId> Create(TEntity data)
    {
        dbContext.Set<TEntity>().Add(data);
        await dbContext.SaveChangesAsync();
        return data.Id;
    }
}