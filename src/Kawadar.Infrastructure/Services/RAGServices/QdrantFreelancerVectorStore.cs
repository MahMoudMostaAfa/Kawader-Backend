using Kawadar.Application.Common.Interfaces;
using Kawadar.Domain.UserProfiles;
using Microsoft.Extensions.Logging;
using Qdrant.Client;
using Qdrant.Client.Grpc;

namespace Kawadar.Infrastructure.Services.RAGServices;

public class QdrantFreelancerVectorStore : IFreelancerVectorStore
{
  private const string CollectionName = "freelancers";
  private readonly QdrantClient _qdrantClient;
  private readonly ILogger<QdrantFreelancerVectorStore> _logger;
  private readonly IEmbeddingService _embeddingService;

  public QdrantFreelancerVectorStore(QdrantClient qdrantClient, ILogger<QdrantFreelancerVectorStore> logger
    , IEmbeddingService embeddingService)
  {
    _qdrantClient = qdrantClient;
    _logger = logger;
    _embeddingService = embeddingService;
  }
  // Implement the methods defined in IFreelancerVectorStore using Qdrant as the vector database.
  public async Task AddFreelancerAsync(UserProfile freelancer)
  {
    _logger.LogInformation("Adding freelancer {FreelancerId} to Qdrant vector store", freelancer.Id);
    var embeddingVector = await _embeddingService.GenerateAsync(freelancer.TextToEmbed);
    await EnsureCollectionExists();


    var point = new PointStruct
    {
      Id = freelancer.Id,
      Vectors = embeddingVector,
      Payload =
      {
        ["Title"] = freelancer.Title ?? "",
        ["Skills"] = freelancer.Skills!= null ? string.Join(",", freelancer.Skills) : "",
        ["Experience"] = freelancer.ExperienceYear.ToString(),
        ["Bio"] = freelancer.Bio ?? ""
      }
    };
    await _qdrantClient.UpsertAsync(CollectionName, [point]);

  }

  public async Task RemoveFreelancerAsync(Guid freelancerId)
  {
    await EnsureCollectionExists();
    throw new NotImplementedException();
  }

  public async Task<List<UserProfile>> SearchFreelancersIdsAsync(string query, int topK)
  {
    await EnsureCollectionExists();
    throw new NotImplementedException();
  }

  public async Task UpdateFreelancerAsync(UserProfile freelancer)
  {
    await EnsureCollectionExists();
    throw new NotImplementedException();
  }

  private async Task EnsureCollectionExists()
  {
    if (!await _qdrantClient.CollectionExistsAsync(CollectionName))
    {
      await _qdrantClient.CreateCollectionAsync(CollectionName, new VectorParams
      {
        Distance = Distance.Cosine
    ,
        Size = 1024
      });
    }
  }
}