using Kawadar.Domain.Common.Results;

namespace Kawadar.Application.Common.Interfaces;

public interface IAIService
{
  Task<Result<T>> GenerateStructuredResponseAsync<T>(
      string prompt,
      CancellationToken ct = default) where T : class;

  Task<Result<T>> GenerateStructuredResponseAsync<T>(
      string prompt,
      IEnumerable<FileData> images,
      CancellationToken ct = default) where T : class;
}


public record FileData(byte[] Data, string MimeType);