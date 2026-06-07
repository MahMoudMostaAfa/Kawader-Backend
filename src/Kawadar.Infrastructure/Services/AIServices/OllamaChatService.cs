using System.Text.Json;
using Kawadar.Application.Common.Interfaces;
using Kawadar.Domain.Common.Results;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.Ollama;


namespace Kawadar.Infrastructure.Services.AIServices;

public class OllamaChatService : IAIChatService
{

  private readonly IChatCompletionService _chatCompletionService;

  public OllamaChatService(IChatCompletionService chatCompletionService)
  {
    _chatCompletionService = chatCompletionService;
  }
  public async Task<Result<T>> ChatAsync<T>(string userMessage, string systemMessage, CancellationToken ct = default)
  {
    try
    {
      var history = new ChatHistory();
      history.AddSystemMessage(systemMessage);
      history.AddUserMessage(userMessage);

      var settings = new OllamaPromptExecutionSettings
      {
        ExtensionData = new Dictionary<string, object> { { "think", false } },
        Temperature = 0.0f
      };

      var response = await _chatCompletionService.GetChatMessageContentAsync(
        history,
        executionSettings: settings,
        cancellationToken: ct);

      if (string.IsNullOrWhiteSpace(response.Content))
      {
        return Error.Failure("AI.EmptyResponse", "The AI returned an empty response.");
      }

      var result = JsonSerializer.Deserialize<T>(response.Content);

      return result is null
        ? Error.Failure("AI.DeserializationFailed", $"Could not deserialize response to {typeof(T).Name}.")
        : result;
    }
    catch (JsonException ex)
    {
      return Error.Failure("AI.InvalidJson", ex.Message);
    }
    catch (Exception ex)
    {
      return Error.Failure("AI.RequestFailed", ex.Message);
    }

  }
}