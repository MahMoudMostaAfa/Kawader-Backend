namespace Kawadar.Application.Common.Interfaces;

public interface IEmbeddingService
{
  Task<float[]> GenerateAsync(string text, CancellationToken ct = default);
}
