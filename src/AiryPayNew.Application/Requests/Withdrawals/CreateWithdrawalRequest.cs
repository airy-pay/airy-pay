using AiryPayNew.Domain.Common;
using AiryPayNew.Domain.Entities.Shops;
using AiryPayNew.Domain.Entities.Withdrawals;
using MediatR;

namespace AiryPayNew.Application.Requests.Withdrawals;

public record CreateWithdrawalRequest(
    ulong ServerId, decimal Amount, string Way, string ReceivingAccountNumber)
    : IRequest<OperationResult>;

public class CreateWithdrawalRequestHandler(
    IWithdrawalRepository withdrawalRepository,
    IShopRepository shopRepository)
    : IRequestHandler<CreateWithdrawalRequest, OperationResult>
{
    private readonly List<string> _withdrawalWays = new() { "card" };
    
    public async Task<OperationResult> Handle(
        CreateWithdrawalRequest request,
        CancellationToken cancellationToken)
    {
        if (request.Amount <= 0)
            return OperationResult.Error("Invalid withdrawal amount.");
        if (string.IsNullOrEmpty(request.Way)
            || string.IsNullOrEmpty(request.ReceivingAccountNumber))
            return OperationResult.Error("Invalid withdrawal details.");
        if (!_withdrawalWays.Contains(request.Way))
            return OperationResult.Error("Invalid withdrawal method.");
        
        var shopId = new ShopId(request.ServerId);
        var shop = await shopRepository.GetByIdNoTrackingAsync(shopId, cancellationToken);
        if (shop is null)
            return OperationResult.Error("Shop not found.");
        
        const int minimalWithdrawalAmount = 500;
        if (request.Amount < minimalWithdrawalAmount)
            return OperationResult.Error($"Minimum withdrawal amount: {minimalWithdrawalAmount} \u20bd");
        if (shop.Balance < request.Amount)
            return OperationResult.Error("Insufficient funds.");

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

        return OperationResult.Success();
    }
}