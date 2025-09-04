namespace AiryPay.Domain.Common.Repositories;

public interface IDeleteRepository<in TId> : IRepository
    where TId : IId
{
    public Task DeleteAsync(TId id, CancellationToken cancellationToken);
}