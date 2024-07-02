namespace AiryPayNew.Domain.Common;

public interface IEntity<TId> where TId : IId
{
    public TId Id { get; set; }
}