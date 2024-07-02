namespace AiryPayNew.Domain.Common;

public class OperationResult<T>
{
    public bool Successful { get; set; }
    public string? ErrorMessage { get; set; }
    public T Entity { get; set; }
    
    public OperationResult(bool successful, T entity)
    {
        Successful = successful;
        Entity = entity;
    }
    
    public OperationResult(bool successful, string errorMessage, T entity)
    {
        Successful = successful;
        ErrorMessage = errorMessage;
        Entity = entity;
    }

    public static OperationResult<T> Success(T entity) => new(true, entity);
}