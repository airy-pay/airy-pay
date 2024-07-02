namespace AiryPayNew.Domain.Common;

public interface IIdBase<T> : IId
{
    public T Value { get; set; }
}