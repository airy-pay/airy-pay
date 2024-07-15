namespace AiryPayNew.Domain.Common.Repositories;

public interface IUpdateRepository<in TId, in TEntity> : IRepository
    where TId : IId
    where TEntity : IEntity<TId>
{
    public Task Update(TEntity entity);
}