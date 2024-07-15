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
    private readonly List<string> _withdrawalWays = [ "card" ];
    
    public async Task<OperationResult> Handle(
        CreateWithdrawalRequest request,
        CancellationToken cancellationToken)
    {
        if (request.Amount <= 0)
            return OperationResult.Error("Некорректная сумма вывода.");
        if (string.IsNullOrEmpty(request.Way)
            || string.IsNullOrEmpty(request.ReceivingAccountNumber))
            return OperationResult.Error("Некорректные реквизиты вывода.");
        if (!_withdrawalWays.Contains(request.Way))
            return OperationResult.Error("Некорректный способ вывода средств.");
        
        var shopId = new ShopId(request.ServerId);
        var shop = await shopRepository.GetByIdNoTrackingAsync(shopId);
        if (shop is null)
            return OperationResult.Error("Магазин не найден.");
        
        const int minimalWithdrawalAmount = 500;
        if (request.Amount < minimalWithdrawalAmount)
            return OperationResult.Error($"Минимальная сумма вывода: {minimalWithdrawalAmount} \u20bd");
        if (shop.Balance < request.Amount)
            return OperationResult.Error("Недостаточно средств.");

        var newWithdrawal = new Withdrawal
        {
            Amount = request.Amount,
            WithdrawalStatus = WithdrawalStatus.InProcess,
            Way = request.Way,
            ReceivingAccountNumber = request.ReceivingAccountNumber,
            DateTime = DateTime.UtcNow,
            ShopId = shop.Id
        };
        await withdrawalRepository.Create(newWithdrawal);

        return OperationResult.Success();
    }
}