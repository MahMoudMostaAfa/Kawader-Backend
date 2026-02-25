using System.Text.Json;
using Google.GenAI;
using Google.GenAI.Types;
using Kawadar.Application.Common.Interfaces;
using Kawadar.Domain.Common.Results;
using Microsoft.Extensions.Configuration;

namespace Kawadar.Infrastructure.Services.AIServices;

public class GeminiApiService : IAIService
{
  private readonly Client _client;
  private const string MODEL_NAME = "gemini-3-flash-preview";

  public GeminiApiService(IConfiguration configuration)
  {
    string apiKey = configuration["Gemini:ApiKey"]!;
    _client = new Client(apiKey: apiKey);
  }
  public async Task<Result<T>> GenrateStructuredResponseAsync<T>(string prompt, object Schema, CancellationToken ct = default) where T : class
  {
    return await ExecuteInternalAsync<T>(new List<Part> { new() { Text = prompt } }, Schema, ct);
  }

  public async Task<Result<T>> GenrateStructuredResponseAsync<T>(string prompt, IEnumerable<Application.Common.Interfaces.FileData> images, Object Schema, CancellationToken ct = default) where T : class
  {
    var parts = new List<Part> { new Part { Text = prompt } };

    foreach (var image in images)
    {
      parts.Add(new Part
      {
        InlineData = new Blob { Data = image.Data, MimeType = image.MimeType }
      });
    }
    return await ExecuteInternalAsync<T>(parts, Schema, ct);
  }

  private async Task<Result<T>> ExecuteInternalAsync<T>(List<Part> parts, object schema, CancellationToken ct = default) where T : class
  {
    var config = new GenerateContentConfig
    {
      ResponseMimeType = "application/json",
      ResponseSchema = (Schema)schema
    };


    var content = new Content { Parts = parts };
    var response = await _client.Models.GenerateContentAsync(MODEL_NAME, content, config, ct);

    var contentText = response.Candidates?[0]?.Content?.Parts[0].Text;

    Console.WriteLine(contentText);


    var result = JsonSerializer.Deserialize<T>(contentText!, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

    if (result is null)
      return Error.Failure();

    return result;

  }
}