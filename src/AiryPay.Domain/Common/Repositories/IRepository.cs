namespace AiryPay.Domain.Common.Repositories;

public interface IDefaultRepository<TId, TEntity> : IRepository
    where TId : IId
    where TEntity : IEntity<TId>
{
    public Task<TEntity?> GetByIdAsync(TId id, CancellationToken cancellationToken);
    public Task<TId> CreateAsync(TEntity data, CancellationToken cancellationToken);
}