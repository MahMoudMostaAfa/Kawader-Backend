using System.Text.Json;
using System.Text.Json.Serialization;
using Google.GenAI;
using Google.GenAI.Types;
using Kawadar.Application.Common.Interfaces;
using Kawadar.Domain.Common.Results;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Kawadar.Infrastructure.Services.AIServices;

public class GeminiApiService : IAIService
{
  private readonly Client _client;
  private readonly ILogger<GeminiApiService> _logger;
  private const string MODEL_NAME = "gemini-3-flash-preview";

  private static readonly JsonSerializerOptions _jsonOptions = new()
  {
    PropertyNameCaseInsensitive = true,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
  };

  public GeminiApiService(IConfiguration configuration, ILogger<GeminiApiService> logger)
  {
    _logger = logger;
    string apiKey = configuration["Gemini:ApiKey"]
        ?? throw new InvalidOperationException("Gemini:ApiKey is not configured.");
    _client = new Client(apiKey: apiKey);
  }

  public Task<Result<T>> GenerateStructuredResponseAsync<T>(
      string prompt, CancellationToken ct = default) where T : class
  {
    var parts = new List<Part> { new() { Text = prompt } };
    return ExecuteInternalAsync<T>(parts, ct);
  }

  public Task<Result<T>> GenerateStructuredResponseAsync<T>(
      string prompt, IEnumerable<Application.Common.Interfaces.FileData> images, CancellationToken ct = default) where T : class
  {
    var parts = new List<Part> { new() { Text = prompt } };

    foreach (var image in images)
      parts.Add(new Part
      {
        InlineData = new Blob { Data = image.Data, MimeType = image.MimeType }
      });

    return ExecuteInternalAsync<T>(parts, ct);
  }

  private async Task<Result<T>> ExecuteInternalAsync<T>(
      List<Part> parts, CancellationToken ct) where T : class
  {
    try
    {
      var schema = SchemaGenerator.FromType<T>();

      var config = new GenerateContentConfig
      {
        ResponseMimeType = "application/json",
        ResponseSchema = schema
      };

      var content = new Content { Parts = parts };
      var response = await _client.Models.GenerateContentAsync(MODEL_NAME, content, config, ct);

      var text = response.Candidates?.FirstOrDefault()?.Content?.Parts?.FirstOrDefault()?.Text;

      if (string.IsNullOrWhiteSpace(text))
      {
        _logger.LogWarning("Gemini returned empty content for type {Type}", typeof(T).Name);
        return Error.Failure("AI.EmptyResponse", "The AI returned an empty response.");
      }

      _logger.LogDebug("Gemini raw response: {Response}", text);

      var result = JsonSerializer.Deserialize<T>(text, _jsonOptions);

      return result is null
          ? Error.Failure("AI.DeserializationFailed", $"Could not deserialize response to {typeof(T).Name}.")
          : result;
    }
    catch (JsonException ex)
    {
      _logger.LogError(ex, "Failed to deserialize Gemini response to {Type}", typeof(T).Name);
      return Error.Failure("AI.InvalidJson", "The AI response was not valid JSON.");
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Gemini API call failed");
      return Error.Failure("AI.RequestFailed", ex.Message);
    }
  }
}