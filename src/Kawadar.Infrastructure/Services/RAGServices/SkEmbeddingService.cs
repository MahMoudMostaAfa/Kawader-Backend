namespace Kawadar.Infrastructure.Services.RAGServices;

using Kawadar.Application.Common.Interfaces;


using Microsoft.Extensions.AI;

public class SkEmbeddingService(
    IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator) : IEmbeddingService
{
  public async Task<float[]> GenerateAsync(string text, CancellationToken ct = default)
  {
    var result = await embeddingGenerator.GenerateVectorAsync(text, cancellationToken: ct);
    return result.ToArray();
  }
}