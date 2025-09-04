using AiryPay.Domain.Common.Result;
using AiryPay.Domain.Entities.Shops;
using AiryPay.Domain.Entities.Withdrawals;
using MediatR;

namespace AiryPay.Application.Requests.Withdrawals;

using Error = CreateWithdrawalRequest.Error;

public record CreateWithdrawalRequest(
    ShopId ShopId,
    decimal Amount,
    string Way,
    string ReceivingAccountNumber)
    : IRequest<Result<CreateWithdrawalRequest.Error>>
{
    public enum Error
    {
        InvalidWithdrawalAmount,
        InvalidWithdrawalDetails,
        InvalidWithdrawalMethod,
        ShopNotFound,
        WithdrawalAmountTooLow,
        InsufficientFunds
    }
}

public class CreateWithdrawalRequestHandler(
    IWithdrawalRepository withdrawalRepository,
    IShopRepository shopRepository)
    : IRequestHandler<CreateWithdrawalRequest, Result<Error>>
{
    private readonly List<string> _withdrawalWays = ["card"];
    
    public async Task<Result<Error>> Handle(
        CreateWithdrawalRequest request,
        CancellationToken cancellationToken)
    {
        var resultBuilder = new ResultBuilder<Error>();
        
        if (request.Amount <= 0)
            return resultBuilder.WithError(Error.InvalidWithdrawalAmount);
        if (string.IsNullOrEmpty(request.Way)
            || string.IsNullOrEmpty(request.ReceivingAccountNumber))
            return resultBuilder.WithError(Error.InvalidWithdrawalDetails);
        if (!_withdrawalWays.Contains(request.Way))
            return resultBuilder.WithError(Error.InvalidWithdrawalMethod);
        
        var shop = await shopRepository.GetByIdNoTrackingAsync(request.ShopId, cancellationToken);
        if (shop is null)
            return resultBuilder.WithError(Error.ShopNotFound);
        
        const int minimalWithdrawalAmount = 500;
        if (request.Amount < minimalWithdrawalAmount)
            return resultBuilder.WithError(Error.WithdrawalAmountTooLow);
        if (shop.Balance < request.Amount)
            return resultBuilder.WithError(Error.InsufficientFunds);

        await shopRepository.UpdateBalanceAsync(shop.Id, -request.Amount, cancellationToken);
        
        var newWithdrawal = new Withdrawal
        {
            Amount = request.Amount,
            WithdrawalStatus = WithdrawalStatus.InProcess,
            Way = request.Way,
            ReceivingAccountNumber = request.ReceivingAccountNumber,
            DateTime = DateTime.UtcNow,
            ShopId = shop.Id
        };
        await withdrawalRepository.CreateAsync(newWithdrawal, cancellationToken);

        return resultBuilder.WithSuccess();
    }
}