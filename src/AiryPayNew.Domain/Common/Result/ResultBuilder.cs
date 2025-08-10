namespace AiryPayNew.Domain.Common.Result;

public class ResultBuilder<TEntity, TErrorType>
    where TEntity : class
    where TErrorType : Enum
{
    private bool _isSuccess = true;
    
    private TErrorType ErrorType { get; set; } = default!;
    private TEntity Entity { get; set; }

    public ResultBuilder(TEntity entity)
    {
        Entity = entity;
    }
    
    public ResultBuilder<TEntity, TErrorType> WithEntity(TEntity entity)
    {
        Entity = entity;
        
        return this;
    }
    
    public ResultBuilder<TEntity, TErrorType> WithSuccess()
    {
        _isSuccess = true;
        
        return this;
    }

    public ResultBuilder<TEntity, TErrorType> WithSuccess(TEntity entity)
    {
        Entity = entity;
        _isSuccess = true;
        
        return this;
    }

    public ResultBuilder<TEntity, TErrorType> WithError(TErrorType errorType)
    {
        ErrorType = errorType;
        _isSuccess = false;
        
        return this;
    }
    
    public ResultBuilder<TEntity, TErrorType> WithError(TEntity entity, TErrorType errorType)
    {
        Entity = entity;
        ErrorType = errorType;
        _isSuccess = false;
        
        return this;
    }
    
    public Result<TEntity, TErrorType> Build()
    {
        if (_isSuccess)
        {
            return Result<TEntity, TErrorType>.Success(Entity);
        }
        
        return Result<TEntity, TErrorType>.Fail(Entity, ErrorType);
    }
    
    public static implicit operator Result<TEntity, TErrorType>(ResultBuilder<TEntity, TErrorType> resultBuilder)
        => resultBuilder.Build();
}

public class ResultBuilder<TErrorType>
    where TErrorType : Enum
{
    private bool _isSuccess = true;
    
    private TErrorType ErrorType { get; set; } = default!;
    
    public ResultBuilder<TErrorType> WithSuccess()
    {
        _isSuccess = true;
        
        return this;
    }

    public ResultBuilder<TErrorType> WithError(TErrorType errorType)
    {
        ErrorType = errorType;
        _isSuccess = false;
        
        return this;
    }
    
    public Result<TErrorType> Build()
    {
        if (_isSuccess)
        {
            return Result<TErrorType>.Success();
        }
        
        return Result<TErrorType>.Fail(ErrorType);
    }
    
    public static implicit operator Result<TErrorType>(ResultBuilder<TErrorType> resultBuilder)
        => resultBuilder.Build();
}