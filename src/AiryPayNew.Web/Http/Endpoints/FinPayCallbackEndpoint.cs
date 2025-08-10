using AiryPayNew.Application.Common;
using AiryPayNew.Application.Requests.Payments;
using AiryPayNew.Domain.Entities.Bills;
using AiryPayNew.Shared.Messaging.Contracts;
using FinPay.API.Callbacks.Impl;
using MediatR;

namespace AiryPayNew.Web.Http.Endpoints;

[Endpoint]
public static class FinPayCallbackEndpoint
{
    public static void AddEndpoint(this WebApplication app)
    {
        app.MapPost("/payments/callbacks/finpay/", async (
            PaymentPaidCallback paymentPaidCallback,
            IMediator mediator,
            ILogger<WebApplication> logger,
            IRoleAssignmentQueueService roleAssignmentQueueService) =>
        {
            paymentPaidCallback.Amount /= 100;
            paymentPaidCallback.InvoiceId -= 100_000;

            logger.LogInformation(
                $"Received a callback for bill #{paymentPaidCallback.InvoiceId} in shop {paymentPaidCallback.ShopId}");

            var getBillRequest = new GetBillRequest(
                new BillId(paymentPaidCallback.InvoiceId));
            var bill = await mediator.Send(getBillRequest);
            if (bill is null || bill.Product.Price > paymentPaidCallback.Amount)
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
                $"Queued role assignment for user {message.UserId} in guild {message.GuildId}, role {message.RoleId}");
        });

    }
}