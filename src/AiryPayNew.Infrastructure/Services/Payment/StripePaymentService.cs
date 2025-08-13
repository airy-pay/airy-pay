namespace AiryPayNew.Infrastructure.Services.Payment;

using Application.Payments;
using Domain.Common.Result;
using Domain.Entities.Bills;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;

public class StripePaymentService : IPaymentService
{
    private const string NameConst = "Stripe";
    private readonly Shared.Settings.AppSettingsNested.PaymentSystems.Stripe _settings;
    private readonly ILogger<StripePaymentService> _logger;

    public StripePaymentService(
        IOptions<Shared.Settings.AppSettingsNested.PaymentSystems.Stripe> settings,
        ILogger<StripePaymentService> logger)
    {
        _settings = settings.Value;
        _logger = logger;
        StripeConfiguration.ApiKey = _settings.ApiKey;
    }

    public string GetServiceName() => NameConst;

    public async Task<Result<string, IPaymentService.Error>> CreateAsync(Bill bill, string paymentMethod)
    {
        try
        {
            return await CreateBillAsync(bill, paymentMethod);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Stripe payment creation failed");
            return Result<string, IPaymentService.Error>.Fail(string.Empty, IPaymentService.Error.Failed);
        }
    }

    private async Task<Result<string, IPaymentService.Error>> CreateBillAsync(Bill bill, string paymentMethod)
    {
        var options = new SessionCreateOptions
        {
            PaymentMethodTypes = [paymentMethod],
            Mode = "payment",
            LineItems = GetPaymentLineItems(bill),
            SuccessUrl = $"{_settings.SuccessUrl}?session_id={{CHECKOUT_SESSION_ID}}",
            CancelUrl = _settings.CancelUrl,
            ClientReferenceId = bill.Id.Value.ToString(),
            Metadata = new Dictionary<string, string>
            {
                ["BillId"] = bill.Id.Value.ToString(),
                ["BillSecret"] = bill.BillSecret.Key
            }
        };

        var service = new SessionService();
        var session = await service.CreateAsync(options);

        if (string.IsNullOrEmpty(session.Url))
            return Result<string, IPaymentService.Error>.Fail(string.Empty, IPaymentService.Error.Failed);

        return Result<string, IPaymentService.Error>.Success(session.Url);
    }

    private static List<SessionLineItemOptions> GetPaymentLineItems(Bill bill)
    {
        return [
            new SessionLineItemOptions
            {
                PriceData = new SessionLineItemPriceDataOptions
                {
                    Currency = "RUB",
                    UnitAmountDecimal = bill.Product.Price * 100,
                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name = bill.Product.Name
                    }
                },
                Quantity = 1
            }
        ];
    }
}
