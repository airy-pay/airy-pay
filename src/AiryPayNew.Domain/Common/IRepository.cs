namespace AiryPayNew.Domain.Common;

public interface IRepository<TId, TEntity>
    where TId : IId
    where TEntity : IEntity<TId>
{
    public Task<TEntity?> GetByIdAsync(TId id);
    public Task<TId> Create(TEntity data);
}