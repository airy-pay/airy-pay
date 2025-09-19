using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using AiryPay.Application.Payments;
using AiryPay.Domain.Common.Result;
using AiryPay.Domain.Entities.Bills;
using AiryPay.Shared.Settings.AppSettingsNested.PaymentSystemsSettings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AiryPay.Infrastructure.Services.Payment;

public class SquarePaymentService : IPaymentService
{
    private const string Name = "Square";
    
    private readonly HttpClient _http;
    private readonly SquareSettings _settings;
    private readonly ILogger<SquarePaymentService> _logger;
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    public SquarePaymentService(
        IOptions<SquareSettings> options,
        HttpClient httpClient,
        ILogger<SquarePaymentService> logger)
    {
        _settings = options.Value;
        _http = httpClient;
        _logger = logger;
    }

    public string GetServiceName() => Name;

    public async Task<Result<string, IPaymentService.Error>> CreateAsync(Bill bill, string paymentMethod)
    {
        try
        {
            return await CreateAsync(bill);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception during Square CreatePaymentLink");
            return Result<string, IPaymentService.Error>.Fail(
                string.Empty, IPaymentService.Error.Failed);
        }
    }

    private async Task<Result<string, IPaymentService.Error>> CreateAsync(Bill bill)
    {
        var idempotencyKey = Guid.NewGuid().ToString();

        var amountMinor = Convert.ToInt64(
            decimal.Round(
                bill.Product.Price * 100m,
                0, 
                MidpointRounding.AwayFromZero));

        var currency = "RUB";

        var payload = new
        {
            idempotency_key = idempotencyKey,
            order = new
            {
                location_id = bill.Shop.Id.Value.ToString(),
                reference_id = bill.Id.Value.ToString(),
                line_items = new[]
                {
                    new {
                        name = string.IsNullOrWhiteSpace(bill.Product.Name) ? "Product" : bill.Product.Name,
                        quantity = "1",
                        base_price_money = new {
                            amount = amountMinor, currency
                        }
                    }
                }
            },
            description = $"#{bill.Id.Value} payment"
        };

        var json = JsonSerializer.Serialize(payload, _jsonOptions);

        var baseUrl = _settings.UseSandbox
            ? "https://connect.squareupsandbox.com"
            : "https://connect.squareup.com";

        using var request = new HttpRequestMessage(
            HttpMethod.Post,
            $"{baseUrl}/v2/online-checkout/payment-links");
            
        request.Content = new StringContent(
            json,
            Encoding.UTF8,
            "application/json");

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _settings.AccessToken);
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        using var resp = await _http.SendAsync(request);
        var body = await resp.Content.ReadAsStringAsync();

        if (!resp.IsSuccessStatusCode)
        {
            _logger.LogError(
                "Square CreatePaymentLink failed. Status: {Status}." +
                "Body: {Body}", resp.StatusCode, body);
            return Result<string, IPaymentService.Error>.Fail(
                string.Empty, IPaymentService.Error.Failed);
        }

        using var doc = JsonDocument.Parse(body);
        
        if (doc.RootElement.TryGetProperty("payment_link", out var linkElement)
            && linkElement.TryGetProperty("url", out var urlElement))
        {
            var url = urlElement.GetString();
            if (!string.IsNullOrEmpty(url))
                return Result<string, IPaymentService.Error>.Success(url);
        }

        _logger.LogError("Square response didn't contain payment_link.url. Body: {Body}", body);
        return Result<string, IPaymentService.Error>.Fail(
            string.Empty, IPaymentService.Error.Failed);
    }
}
