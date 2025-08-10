namespace AiryPayNew.Domain.Common.Result;

public interface IErrorResult<out TErrorType> : IResult
    where TErrorType : Enum
{
    public TErrorType? ErrorType { get; }
}