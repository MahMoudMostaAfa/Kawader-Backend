using Kawadar.Domain.Common.Results;

namespace Kawadar.Application.Common.Interfaces;

public interface IAIChatService
{
  Task<Result<T>> ChatAsync<T>(string userMessage, string systemMessage, CancellationToken ct = default);
}