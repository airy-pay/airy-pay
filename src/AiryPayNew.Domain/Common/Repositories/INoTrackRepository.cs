namespace AiryPayNew.Domain.Common.Repositories;

public interface INoTrackRepository<in TId, TEntity> : IRepository
    where TId : IId
    where TEntity : IEntity<TId>
{
    public Task<TEntity?> GetByIdNoTrackingAsync(TId id);
}