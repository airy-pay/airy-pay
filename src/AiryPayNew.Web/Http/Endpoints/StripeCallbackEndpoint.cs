using AiryPayNew.Application.Common;
using AiryPayNew.Application.Requests.Payments;
using AiryPayNew.Domain.Entities.Bills;
using AiryPayNew.Shared.Messaging.Contracts;
using MediatR;
using Stripe;
using Stripe.Checkout;

namespace AiryPayNew.Web.Http.Endpoints;

[Endpoint]
public static class StripeCallbackEndpoint
{
    public static void AddEndpoint(this WebApplication app)
    {
        app.MapPost("/payments/callbacks/stripe/", async (
            HttpRequest request,
            IMediator mediator,
            ILogger<WebApplication> logger,
            IRoleAssignmentQueueService roleAssignmentQueueService) =>
        {
            string json;
            using (var reader = new StreamReader(request.Body))
            {
                json = await reader.ReadToEndAsync();
            }

            var endpointSecret = Environment.GetEnvironmentVariable("STRIPE_WEBHOOK_SECRET")
                                 ?? throw new InvalidOperationException("Stripe webhook secret not configured");

            Event stripeEvent;
            try
            {
                var signatureHeader = request.Headers["Stripe-Signature"];
                stripeEvent = EventUtility.ConstructEvent(json, signatureHeader, endpointSecret);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Stripe webhook signature verification failed");
                return Results.BadRequest();
            }

            if (stripeEvent.Type != "CheckoutSessionCompleted")
                return Results.Ok();
            
            var session = stripeEvent.Data.Object as Session;
            if (session == null)
                return Results.Ok();

            if (!int.TryParse(session.ClientReferenceId, out var billId))
            {
                logger.LogWarning($"Invalid BillId in Stripe session: {session.ClientReferenceId}");
                return Results.Ok();
            }

            logger.LogInformation($"Received Stripe payment callback for bill #{billId}");

            var getBillRequest = new GetBillRequest(new BillId(billId));
            var bill = await mediator.Send(getBillRequest);
            if (bill is null)
                return Results.Ok();

            var completePaymentRequest = new CompletePaymentRequest(bill.Id);
            var completePaymentResult = await mediator.Send(completePaymentRequest);
            if (!completePaymentResult.Successful)
                return Results.Ok();

            if (bill.Product.DiscordRoleId is null)
                return Results.Ok();

            var message = new AssignRoleMessage(
                (ulong)bill.Product.Shop.Id.Value,
                bill.BuyerDiscordId,
                bill.Product.DiscordRoleId.Value,
                bill.Id);

            await roleAssignmentQueueService.EnqueueAsync(message);

            logger.LogInformation(
                $"Queued Stripe role assignment for user {message.UserId} in guild {message.GuildId}, role {message.RoleId}");

            return Results.Ok();
        });
    }
}
