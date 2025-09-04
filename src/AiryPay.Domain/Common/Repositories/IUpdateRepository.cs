namespace AiryPay.Domain.Common.Repositories;

public interface IUpdateRepository<in TId, in TEntity> : IRepository
    where TId : IId
    where TEntity : IEntity<TId>
{
    public Task UpdateAsync(TEntity entity, CancellationToken cancellationToken);
}