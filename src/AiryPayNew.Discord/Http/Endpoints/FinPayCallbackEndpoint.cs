using AiryPayNew.Application.Requests.Payments;
using AiryPayNew.Discord.Services;
using Discord.WebSocket;
using FinPay.API.Callbacks.Impl;
using MediatR;

namespace AiryPayNew.Discord.Http.Endpoints;

[Endpoint]
public static class FinPayCallbackEndpoint
{
    public static void AddEndpoint(this WebApplication app)
    {
        app.MapPost("/payments/callbacks/finpay/", async (
            PaymentPaidCallback paymentPaidCallback,
            IMediator mediator,
            DiscordSocketClient discordSocketClient,
            UserRepositoryService socketUserRepositoryService,
            ILogger<WebApplication> logger) =>
        {
            // FinPay's API is ASS
            paymentPaidCallback.Amount /= 100;
            paymentPaidCallback.InvoiceId -= 100_000;
            
            logger.LogInformation(
                string.Format("Received a callback ({0}) for bill #{1} in shop {2}",
                    nameof(FinPayCallbackEndpoint),
                    paymentPaidCallback.InvoiceId,
                    paymentPaidCallback.ShopId));
            
            var getBillRequest = new GetBillRequest(paymentPaidCallback.InvoiceId);
            var bill = await mediator.Send(getBillRequest);
            if (bill is null)
                return;
            if (bill.Product.Price > paymentPaidCallback.Amount)
                return;
            
            var completePaymentRequest = new CompletePaymentRequest(bill.Id);
            var completePaymentResult = await mediator.Send(completePaymentRequest);
            if (!completePaymentResult.Successful)
                return;
            
            var guild = discordSocketClient.GetGuild((ulong) bill.Product.Shop.Id.Value);
            var user = socketUserRepositoryService.GetUser(bill.BuyerDiscordId) as SocketGuildUser;
            if (user is null)
                return;

            if (bill.Product.DiscordRoleId is null)
                return;
            var role = guild.GetRole(bill.Product.DiscordRoleId.Value);
            if (role is null)
                return;

            await user.AddRoleAsync(role);
            
            logger.LogInformation(
                string.Format("Gave a role to user {0} for bill #{1} in shop {2}",
                    user.Id,
                    paymentPaidCallback.InvoiceId,
                    paymentPaidCallback.ShopId));
        });
    }
}