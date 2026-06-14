using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Kawadar.Application.Common.Interfaces;
using Kawadar.Domain.Common.Results;
using Kawadar.Infrastructure.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Kawadar.Infrastructure.Services.PaymentServices;

public class PaymobService : IPaymobService
{
  private readonly HttpClient _httpClient;
  private readonly PaymobSettings _settings;
  private readonly ILogger<PaymobService> _logger;

  private static readonly JsonSerializerOptions JsonOptions = new()
  {
    PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    PropertyNameCaseInsensitive = true
  };

  public PaymobService(HttpClient httpClient, IOptions<PaymobSettings> settings, ILogger<PaymobService> logger)
  {
    _httpClient = httpClient;
    _settings = settings.Value;
    _logger = logger;
  }


  public async Task<Result<PaymobIntentionResult>> CreatePaymentIntentionAsync(
    decimal amount,
    string currency,
    List<string> paymentMethodIds,
    PaymobBillingData billingData,
    string? internalOrderId = null,
    CancellationToken ct = default)
  {
    try
    {
      // Paymob expects amount in cents (smallest currency unit)
      var amountCents = (int)(amount * 100);

      var requestBody = new
      {
        amount = amountCents,
        currency,
        payment_methods = paymentMethodIds,
        billing_data = new
        {
          first_name = billingData.FirstName,
          last_name = billingData.LastName,
          email = billingData.Email,
          phone_number = billingData.PhoneNumber,
          apartment = billingData.Apartment ?? "NA",
          floor = billingData.Floor ?? "NA",
          street = billingData.Street ?? "NA",
          building = billingData.Building ?? "NA",
          shipping_method = billingData.ShippingMethod ?? "NA",
          postal_code = billingData.PostalCode ?? "NA",
          city = billingData.City ?? "NA",
          country = billingData.Country ?? "EG",
          state = billingData.State ?? "NA"
        },
        special_reference = internalOrderId
      };

      var json = JsonSerializer.Serialize(requestBody, JsonOptions);
      var content = new StringContent(json, Encoding.UTF8, "application/json");

      var request = new HttpRequestMessage(HttpMethod.Post, $"{_settings.BaseUrl}/v1/intention/")
      {
        Content = content
      };
      request.Headers.Authorization = new AuthenticationHeaderValue("Token", _settings.SecretKey);

      var response = await _httpClient.SendAsync(request, ct);

      var responseBody = await response.Content.ReadAsStringAsync(ct);

      if (!response.IsSuccessStatusCode)
      {
        _logger.LogError("Paymob create intention failed. Status: {StatusCode}, Response: {Response}",
          response.StatusCode, responseBody);
        return Error.Failure("Paymob.IntentionFailed", $"Failed to create payment intention. Status: {response.StatusCode}");
      }

      var result = JsonSerializer.Deserialize<PaymobIntentionResponse>(responseBody, JsonOptions);

      if (result is null || string.IsNullOrEmpty(result.ClientSecret))
      {
        _logger.LogError("Paymob returned invalid intention response: {Response}", responseBody);
        return Error.Failure("Paymob.InvalidResponse", "Paymob returned an invalid response.");
      }

      _logger.LogInformation("Paymob payment intention created successfully. IntentionId: {IntentionId}", result.Id);

      return new PaymobIntentionResult(
        IntentionId: result.Id ?? string.Empty,
        ClientSecret: result.ClientSecret,
        Amount: amount,
        Currency: currency);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Exception occurred while creating Paymob payment intention");
      return Error.Unexpected("Paymob.Exception", "An unexpected error occurred while processing the payment.");
    }
  }


  public bool VerifyHmacSignature(PaymobCallbackData data, string receivedHmac)
  {
    try
    {
      // Paymob HMAC is calculated by concatenating these fields in order (values only)
      var concatenated = string.Concat(
        data.AmountCents,
        data.CreatedAt,
        data.Currency,
        data.ErrorCode.ToLower(),
        data.has_parent_transaction.ToString().ToLower(),
        data.transactionId,
        data.IntegrationId,
        data.Is3dSecure.ToString().ToLower(),
        data.IsAuth.ToString().ToLower(),
        data.IsCapture.ToString().ToLower(),
        data.IsRefunded.ToString().ToLower(),
        data.IsStandalonePayment.ToString().ToLower(),
        data.IsVoided.ToString().ToLower(),
        data.OrderId,
        data.Owner,
        data.Pending.ToString().ToLower(),
        data.SourceDataPan,
        data.SourceDataSubType,
        data.SourceDataType,
        data.Success.ToString().ToLower()
      );

      var keyBytes = Encoding.UTF8.GetBytes(_settings.HMAC);
      var messageBytes = Encoding.UTF8.GetBytes(concatenated);

      using var hmac = new HMACSHA512(keyBytes);
      var hashBytes = hmac.ComputeHash(messageBytes);
      var calculatedHmac = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();

      var isValid = string.Equals(calculatedHmac, receivedHmac, StringComparison.OrdinalIgnoreCase);

      if (!isValid)
      {
        _logger.LogWarning("Paymob HMAC verification failed. Expected: {Expected}, Received: {Received}",
          calculatedHmac, receivedHmac);
      }

      return isValid;
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Exception occurred during HMAC verification");
      return false;
    }
  }


  // ─── Internal response DTOs ───

  private sealed class PaymobIntentionResponse
  {
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("client_secret")]
    public string? ClientSecret { get; set; }

    [JsonPropertyName("amount")]
    public int Amount { get; set; }

    [JsonPropertyName("currency")]
    public string? Currency { get; set; }

    [JsonPropertyName("status")]
    public string? Status { get; set; }
  }
}
