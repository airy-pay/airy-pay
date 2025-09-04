namespace AiryPay.Domain.Common.Result;

public interface IResult
{
    public bool Successful { get; }
    public bool Failed { get; }
}