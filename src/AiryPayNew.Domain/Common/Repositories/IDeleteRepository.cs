namespace AiryPayNew.Domain.Common.Repositories;

public interface IDeleteRepository<in TId> : IRepository
    where TId : IId
{
    public Task Delete(TId id);
}