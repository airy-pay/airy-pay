namespace AiryPayNew.Domain.Common.Result;

public readonly struct Result<TEntity, TErrorType>
    : IEntityResult<TEntity>, IErrorResult<TErrorType>
    where TEntity : class
    where TErrorType : Enum
{
    public bool Successful { get; } = false;

    public bool Failed => !Successful;
    
    public TEntity Entity { get; }
    public TErrorType? ErrorType { get; }

    public Result(bool successful, TEntity entity)
    {
        Successful = successful;
        Entity = entity;
    }
    
    public Result(bool successful, TEntity entity, TErrorType errorType)
    {
        Successful = successful;
        Entity = entity;
        ErrorType = errorType;
    }

    public static Result<TEntity, TErrorType> Success(TEntity entity)
        => new(true, entity);
    
    public static Result<TEntity, TErrorType> Fail(TEntity entity, TErrorType error)
        => new(false, entity, error);
}

public readonly struct Result<TErrorType> : IErrorResult<TErrorType>
    where TErrorType : Enum
{
    public bool Successful { get; } = false;

    public bool Failed => !Successful;
    
    public TErrorType? ErrorType { get; }

    public Result(bool successful)
    {
        Successful = successful;
    }

    public Result(bool successful, TErrorType errorType)
    {
        Successful = successful;
        ErrorType = errorType;
    }

    public static Result<TErrorType> Success()
        => new(true);
    
    public static Result<TErrorType> Fail(TErrorType errorType)
        => new(false, errorType);
}

public readonly struct Result : IResult
{
    public bool Successful { get; } = false;

    public bool Failed => !Successful;
    
    public Result(bool successful)
    {
        Successful = successful;
    }

    public static Result Success()
        => new(true);
    
    public static Result Fail()
        => new(false);
}