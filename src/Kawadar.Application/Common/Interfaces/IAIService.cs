using Kawadar.Domain.Common.Results;


namespace Kawadar.Application.Common.Interfaces;

public interface IAIService
{

  Task<Result<T>> GenrateStructuredResponseAsync<T>(string prompt, Object Schema, CancellationToken ct = default) where T : class;

  Task<Result<T>> GenrateStructuredResponseAsync<T>(string prompt, IEnumerable<FileData> files, Object Schema, CancellationToken ct = default) where T : class;

}

public record FileData(byte[] Data, string MimeType);