namespace AiryPayNew.Domain.Common;

public class OperationResult<T>
{
    public T Entity { get; set; }
    public bool Successful { get; set; }
    public string? ErrorMessage { get; set; }
    
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
    public static OperationResult<T> Error(T entity, string errorMessage) => new(false, errorMessage, entity);
}

public class OperationResult
{
    public bool Successful { get; set; }
    public string? ErrorMessage { get; set; }
    
    public OperationResult(bool successful)
    {
        Successful = successful;
    }
    
    public OperationResult(bool successful, string errorMessage)
    {
        Successful = successful;
        ErrorMessage = errorMessage;
    }

    public static OperationResult Success() => new(true);
    public static OperationResult Error(string errorMessage) => new(false, errorMessage);
}