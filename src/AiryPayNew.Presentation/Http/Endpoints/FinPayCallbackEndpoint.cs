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
            UserRepositoryService socketUserRepositoryService) =>
        {
            // FinPay's API is ASS
            paymentPaidCallback.Amount /= 100;
            paymentPaidCallback.InvoiceId -= 100_000;
            
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
        });
    }
}