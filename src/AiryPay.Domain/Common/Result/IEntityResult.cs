namespace AiryPay.Domain.Common.Result;

public interface IEntityResult<out TEntity> : IResult
    where TEntity : class
{
    public TEntity Entity { get; }
}