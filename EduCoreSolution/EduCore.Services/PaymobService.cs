using EduCore.Shared.Settings;
using Microsoft.Extensions.Options;
using RestSharp;
using System.Text.Json;

namespace EduCore.Services
{
    public class PaymobService
    {
        private readonly RestClient _client;
        private readonly PaymobSettings _paymobSettings;

        public PaymobService(IOptions<PaymobSettings> paymobSettings)
        {
            _paymobSettings = paymobSettings.Value;
            _client = new RestClient("https://accept.paymob.com/api");
        }

        public async Task<string> CreateCheckoutAsync(int paymentId, decimal amount, string currency, string studentEmail)
        {
            try
            {
                // 1. Get Authentication Token
                var authToken = await GetAuthTokenAsync();

                // 2. Create Order
                var orderId = await CreateOrderAsync(authToken, amount, currency, paymentId);

                // 3. Generate Payment Key
                var paymentKey = await GetPaymentKeyAsync(authToken, orderId, amount, currency, studentEmail);

                // 4. Return Checkout URL
                var iframeId = _paymobSettings.IFrameId;
                return $"https://accept.paymob.com/api/acceptance/iframes/{iframeId}?payment_token={paymentKey}";
            }
            catch (Exception ex)
            {
                throw new Exception($"Paymob Integration Error: {ex.Message}");
            }
        }

        private async Task<string> GetAuthTokenAsync()
        {
            var request = new RestRequest("/auth/tokens", Method.Post);
            request.AddJsonBody(new
            {
                api_key = _paymobSettings.ApiKey
            });

            var response = await _client.ExecuteAsync(request);

            if (!response.IsSuccessful)
                throw new Exception($"Auth failed: {response.ErrorMessage}");

            var result = JsonSerializer.Deserialize<JsonElement>(response.Content!);
            return result.GetProperty("token").GetString()!;
        }

        private async Task<int> CreateOrderAsync(string authToken, decimal amount, string currency, int paymentId)
        {
            var uniqueOrderId = $"{paymentId}_{DateTime.UtcNow.Ticks}";
            var request = new RestRequest("/ecommerce/orders", Method.Post);
            request.AddJsonBody(new
            {
                auth_token = authToken,
                delivery_needed = false,
                amount_cents = (int)(amount * 100), // Convert to cents
                currency = currency.ToUpper(),
                merchant_order_id = uniqueOrderId,
                items = new object[] { }
            });

            var response = await _client.ExecuteAsync(request);

            if (!response.IsSuccessful)
                throw new Exception($"Order creation failed: {response.Content} | Status: {response.StatusCode}");

            var result = JsonSerializer.Deserialize<JsonElement>(response.Content!);
            return result.GetProperty("id").GetInt32();
        }

        private async Task<string> GetPaymentKeyAsync(string authToken, int orderId, decimal amount, string currency, string email)
        {
            var request = new RestRequest("/acceptance/payment_keys", Method.Post);
            request.AddJsonBody(new
            {
                auth_token = authToken,
                amount_cents = (int)(amount * 100),
                expiration = 3600, // 1 hour
                order_id = orderId,
                currency = currency.ToUpper(),
                integration_id = _paymobSettings.IntegrationId,
                billing_data = new
                {
                    apartment = "NA",
                    email = email,
                    floor = "NA",
                    first_name = "Student",
                    street = "NA",
                    building = "NA",
                    phone_number = "+20100000000",
                    shipping_method = "NA",
                    postal_code = "NA",
                    city = "Cairo",
                    country = "EG",
                    last_name = "User",
                    state = "Cairo"
                }
            });

            var response = await _client.ExecuteAsync(request);

            if (!response.IsSuccessful)
                throw new Exception($"Payment key failed: {response.ErrorMessage}");

            var result = JsonSerializer.Deserialize<JsonElement>(response.Content!);
            return result.GetProperty("token").GetString()!;
        }
    }
}