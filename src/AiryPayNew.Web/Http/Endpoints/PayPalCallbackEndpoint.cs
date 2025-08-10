using AiryPayNew.Application.Common;
using AiryPayNew.Application.Requests.Payments;
using AiryPayNew.Shared.Messaging.Contracts;
using MediatR;

namespace AiryPayNew.Web.Http.Endpoints;

public class PayPalPaymentCompletedCallback
{
    public string InvoiceId { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "USD";
    public string TransactionId { get; set; } = string.Empty;
    public string PayerId { get; set; } = string.Empty;
    public string ShopId { get; set; } = string.Empty;
}

[Endpoint]
public static class PayPalCallbackEndpoint
{
    public static void AddEndpoint(this WebApplication app)
    {
        app.MapPost("/payments/callbacks/paypal/", async (
            PayPalPaymentCompletedCallback callback,
            IMediator mediator,
            ILogger<WebApplication> logger,
            IRoleAssignmentQueueService roleAssignmentQueueService) =>
        {
            logger.LogInformation(
                $"Received PayPal callback for bill #{callback.InvoiceId} (transaction {callback.TransactionId})");

            if (!int.TryParse(callback.InvoiceId, out var billId))
            {
                logger.LogWarning($"Invalid invoice ID in PayPal callback: {callback.InvoiceId}");
                return;
            }

            var getBillRequest = new GetBillRequest(billId);
            var bill = await mediator.Send(getBillRequest);
            if (bill is null || bill.Product.Price > callback.Amount)
                return;

            var completePaymentRequest = new CompletePaymentRequest(bill.Id);
            var completePaymentResult = await mediator.Send(completePaymentRequest);
            if (!completePaymentResult.Successful)
                return;

            if (bill.Product.DiscordRoleId is null)
                return;

            var message = new AssignRoleMessage(
                (ulong)bill.Product.Shop.Id.Value,
                bill.BuyerDiscordId,
                bill.Product.DiscordRoleId.Value,
                bill.Id);

            await roleAssignmentQueueService.EnqueueAsync(message);

            logger.LogInformation(
                $"Queued PayPal role assignment for user {message.UserId} in guild {message.GuildId}, role {message.RoleId}");
        });
    }
}
