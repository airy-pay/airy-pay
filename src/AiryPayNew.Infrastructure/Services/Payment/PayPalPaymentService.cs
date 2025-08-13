using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using AiryPayNew.Application.Payments;
using AiryPayNew.Domain.Common.Result;
using AiryPayNew.Domain.Entities.Bills;
using AiryPayNew.Shared.Settings.AppSettingsNested;
using AiryPayNew.Shared.Settings.AppSettingsNested.PaymentSystems;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AiryPayNew.Infrastructure.Services.Payment;

public class PayPalPaymentService : IPaymentService
    {
        private const string NameConst = "PayPal";
        private readonly PayPal _settings;
        private readonly HttpClient _http;
        private readonly ILogger<PayPalPaymentService> _logger;

        public PayPalPaymentService(
            IOptions<PayPal> settings,
            HttpClient httpClient,
            ILogger<PayPalPaymentService> logger)
        {
            _settings = settings.Value;
            _http = httpClient;
            _logger = logger;
        }

        public string GetServiceName() => NameConst;

        public async Task<Result<string, IPaymentService.Error>> CreateAsync(Bill bill, string paymentMethod)
        {
            var tokenResp = await GetAccessTokenAsync();
            if (tokenResp is null)
            {
                _logger.LogError("Failed to get PayPal access token");
                return Result<string, IPaymentService.Error>.Fail(string.Empty, IPaymentService.Error.Failed);
            }

            var createOrderResult = await CreateOrderAsync(bill, tokenResp);
            if (createOrderResult == null || string.IsNullOrEmpty(createOrderResult))
            {
                _logger.LogError("Failed to create PayPal order");
                return Result<string, IPaymentService.Error>.Fail(string.Empty, IPaymentService.Error.Failed);
            }

            return Result<string, IPaymentService.Error>.Success(createOrderResult);
        }

        private async Task<string?> GetAccessTokenAsync()
        {
            var url = _settings.Sandbox
                ? "https://api.sandbox.paypal.com/v1/oauth2/token"
                : "https://api.paypal.com/v1/oauth2/token";

            var auth = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_settings.ClientId}:{_settings.Secret}"));
            using var req = new HttpRequestMessage(HttpMethod.Post, url);
            req.Headers.Authorization = new AuthenticationHeaderValue("Basic", auth);
            req.Content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["grant_type"] = "client_credentials"
            });

            using var resp = await _http.SendAsync(req);
            if (!resp.IsSuccessStatusCode) return null;

            using var stream = await resp.Content.ReadAsStreamAsync();
            var body = await JsonSerializer.DeserializeAsync<JsonElement>(stream);
            if (body.TryGetProperty("access_token", out var token)
                && body.TryGetProperty("token_type", out var type))
            {
                return token.GetString()!;
            }

            return null;
        }

        private async Task<string?> CreateOrderAsync(Bill bill, string accessToken)
        {
            var url = _settings.Sandbox
                ? "https://api.sandbox.paypal.com/v2/checkout/orders"
                : "https://api.paypal.com/v2/checkout/orders";

            var orderReq = new
            {
                intent = "CAPTURE",
                purchase_units = new[]
                {
                    new
                    {
                        reference_id = bill.Id.Value.ToString(),
                        amount = new { currency_code = "RUB", value = bill.Product.Price.ToString("F2") }
                    }
                },
                application_context = new
                {
                    return_url = "https://your-site.com/paypal/return",
                    cancel_url = "https://your-site.com/paypal/cancel"
                }
            };

            using var req = new HttpRequestMessage(HttpMethod.Post, url);
            req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            req.Content = new StringContent(JsonSerializer.Serialize(orderReq), Encoding.UTF8, "application/json");

            using var resp = await _http.SendAsync(req);
            if (!resp.IsSuccessStatusCode) return null;

            using var stream = await resp.Content.ReadAsStreamAsync();
            var body = await JsonSerializer.DeserializeAsync<JsonElement>(stream);

            if (body.TryGetProperty("links", out var links) && links.ValueKind == JsonValueKind.Array)
            {
                foreach (var link in links.EnumerateArray())
                {
                    if (link.GetProperty("rel").GetString() == "approve")
                        return (link.GetProperty("href").GetString()!);
                }
            }

            return null;
        }
    }