using Gorse.NET.Models;
using Kawadar.Application.Common.Interfaces;
using Kawadar.Application.Common.Interfaces.Repositories;
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
  private readonly IUsersRepository _usersRepository;

  public QdrantFreelancerVectorStore(QdrantClient qdrantClient, ILogger<QdrantFreelancerVectorStore> logger
    , IEmbeddingService embeddingService
    , IUsersRepository usersRepository)
  {
    _qdrantClient = qdrantClient;
    _logger = logger;
    _embeddingService = embeddingService;
    _usersRepository = usersRepository;
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
    await _qdrantClient.DeleteAsync(CollectionName, freelancerId);
  }



  public async Task UpdateFreelancerAsync(UserProfile freelancer)
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



  public async Task<Domain.Common.Results.Result<List<UserProfile>>> SearchFreelancersIdsAsync(string query, int topK)
  {
    await EnsureCollectionExists();
    var embeddingVector = await _embeddingService.GenerateAsync(query);
    var searchResult = await _qdrantClient.SearchAsync(CollectionName, embeddingVector, limit: (ulong)topK, scoreThreshold: 0.5f);
    var ids = searchResult.Select(r => Guid.Parse(r.Id.Uuid)).ToList();
    // dictonary to hold the id and score

    var idswithscores = searchResult.ToDictionary(r => Guid.Parse(r.Id.Uuid), r => r.Score);

    var freelancersResult = await _usersRepository.GetUsersbyIds(ids);

    if (freelancersResult.IsError) return freelancersResult.Errors;
    var freelancers = freelancersResult.Value;
    _logger.LogInformation("Search query: {Query}, Found freelancers: {Count}", query, freelancers.Count());
    // order the freelancers based on the score + avg rating (if exists) to boost the ones with higher ratings
    var orderedFreelancers = freelancers.OrderByDescending(f =>
    {
      var score = idswithscores.ContainsKey(f.Id) ? idswithscores[f.Id] : 0;
      var ratingBoost = f.Reviews != null && f.Reviews.Count > 0 ? f.Reviews.Average(r => r.Rating) / 5 : 0;
      return 0.7f * score + 0.3f * ratingBoost;
    }).ToList();

    return orderedFreelancers;
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