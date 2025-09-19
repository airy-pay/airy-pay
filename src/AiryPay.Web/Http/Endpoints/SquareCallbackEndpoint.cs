using AiryPay.Application.Common;
using AiryPay.Application.Requests.Payments;
using AiryPay.Domain.Entities.Bills;
using AiryPay.Shared.Messaging.Contracts;
using MediatR;

namespace AiryPay.Web.Http.Endpoints;

public class SquarePaymentCompletedCallback
{
    public string Type { get; set; } = string.Empty;
    public SquarePaymentData Data { get; set; } = new();
}

public class SquarePaymentData
{
    public SquarePaymentObject Object { get; set; } = new();
}

public class SquarePaymentObject
{
    public SquarePayment Payment { get; set; } = new();
}

public class SquarePayment
{
    public string Id { get; set; } = string.Empty;
    public string OrderId { get; set; } = string.Empty;
    public Money AmountMoney { get; set; } = new();
}

public class Money
{
    public long Amount { get; set; }
    public string Currency { get; set; } = "USD";
}

[Endpoint]
public static class SquareCallbackEndpoint
{
    public static void AddEndpoint(this WebApplication app)
    {
        app.MapPost("/payments/callbacks/square/", async (
            SquarePaymentCompletedCallback callback,
            IMediator mediator,
            ILogger<WebApplication> logger,
            IRoleAssignmentQueueService roleAssignmentQueueService) =>
        {
            if (callback.Type != "payment.updated")
                return;

            var payment = callback.Data.Object.Payment;
            if (!int.TryParse(payment.OrderId, out var billId))
            {
                logger.LogWarning("Invalid order ID in Square callback: {OrderId}", payment.OrderId);
                return;
            }

            var amountDecimal = payment.AmountMoney.Amount / 100m;

            logger.LogInformation(
                "Received Square callback for bill #{BillId} (payment {PaymentId})",
                billId, payment.Id);

            var getBillRequest = new GetBillRequest(new BillId(billId));
            var bill = await mediator.Send(getBillRequest);
            if (bill is null || bill.Product.Price > amountDecimal)
                return;

            var completePaymentRequest = new CompletePaymentRequest(bill.Id);
            var completePaymentResult = await mediator.Send(completePaymentRequest);
            if (!completePaymentResult.Successful)
                return;

            if (bill.Product.DiscordRoleId is null)
                return;

            var message = new AssignRoleMessage(
                (ulong) bill.Product.Shop.Id.Value,
                bill.BuyerDiscordId,
                bill.Product.DiscordRoleId.Value,
                bill.Id);

            await roleAssignmentQueueService.EnqueueAsync(message);

            logger.LogInformation(
                "Queued Square role assignment for user {UserId} in guild {GuildId}, role {RoleId}",
                message.UserId, message.GuildId, message.RoleId);
        });
    }
}