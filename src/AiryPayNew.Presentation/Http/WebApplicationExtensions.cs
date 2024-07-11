using FinPay.API.Callbacks.Impl;
using MediatR;

namespace AiryPayNew.Discord.Http;

public static class WebApplicationExtensions
{
    public static void AddEndpoints(this WebApplication app)
    {
        app.MapPost("/payments/callbacks/finpay/", 
            async (PaymentPaidCallback paymentPaidCallback, IMediator mediator) =>
        {
            
            
            return Results.Ok();
        });
    }
}