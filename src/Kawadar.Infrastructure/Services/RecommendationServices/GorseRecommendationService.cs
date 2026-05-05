using Gorse.NET.Models;
using Gorse.NET.Utilities;
using Kawadar.Application.Common.Interfaces;
using Kawadar.Infrastructure.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using GorseClient = Gorse.NET.Gorse;
using DomainResult = Kawadar.Domain.Common.Results.Result;
using DomainError = Kawadar.Domain.Common.Results.Error;

namespace Kawadar.Infrastructure.Services.RecommendationServices;

public class GorseRecommendationService : IRecommendationService
{
  private readonly GorseClient _client;
  private readonly ILogger<GorseRecommendationService> _logger;

  public GorseRecommendationService(IOptions<GorseSettings> options, ILogger<GorseRecommendationService> logger)
  {
    _logger = logger;
    var settings = options.Value;

    if (string.IsNullOrWhiteSpace(settings.BaseUri))
      throw new InvalidOperationException("Gorse:BaseUri is not configured.");
    if (string.IsNullOrWhiteSpace(settings.ApiKey))
      throw new InvalidOperationException("Gorse:ApiKey is not configured.");

    _client = new GorseClient(settings.BaseUri, settings.ApiKey);
  }

  // ─── Users ───────────────────────────────────

  public async Task<Domain.Common.Results.Result<Domain.Common.Results.Success>> InsertUserAsync(string userId, object? labels = null, string? comment = null, CancellationToken ct = default)
  {
    try
    {
      await _client.InsertUserAsync(new User
      {
        UserId = userId,
        Labels = labels,
        Comment = comment ?? ""
      });

      _logger.LogInformation("Inserted user {UserId} into Gorse", userId);
      return DomainResult.Success;
    }
    catch (GorseException ex) { return HandleException(ex, "InsertUser"); }
    catch (Exception ex) { return HandleUnexpected(ex, "InsertUser"); }
  }

  public async Task<Domain.Common.Results.Result<Domain.Common.Results.Success>> InsertUsersAsync(IEnumerable<RecommendationUser> users, CancellationToken ct = default)
  {
    try
    {
      var gorseUsers = users.Select(u => new User
      {
        UserId = u.UserId,
        Labels = u.Labels,
        Comment = u.Comment ?? ""
      }).ToList();

      await _client.InsertUsersAsync(gorseUsers);

      _logger.LogInformation("Inserted {Count} users into Gorse", gorseUsers.Count);
      return DomainResult.Success;
    }
    catch (GorseException ex) { return HandleException(ex, "InsertUsers"); }
    catch (Exception ex) { return HandleUnexpected(ex, "InsertUsers"); }
  }

  public async Task<Domain.Common.Results.Result<RecommendationUser>> GetUserAsync(string userId, CancellationToken ct = default)
  {
    try
    {
      var user = await _client.GetUserAsync(userId);
      return new RecommendationUser(user.UserId, user.Labels, user.Comment);
    }
    catch (GorseException ex) { return HandleException(ex, "GetUser"); }
    catch (Exception ex) { return HandleUnexpected(ex, "GetUser"); }
  }

  public async Task<Domain.Common.Results.Result<Domain.Common.Results.Deleted>> DeleteUserAsync(string userId, CancellationToken ct = default)
  {
    try
    {
      await _client.DeleteUserAsync(userId);
      _logger.LogInformation("Deleted user {UserId} from Gorse", userId);
      return DomainResult.Deleted;
    }
    catch (GorseException ex) { return HandleException(ex, "DeleteUser"); }
    catch (Exception ex) { return HandleUnexpected(ex, "DeleteUser"); }
  }

  // ─── Items ───────────────────────────────────

  public async Task<Domain.Common.Results.Result<Domain.Common.Results.Success>> InsertItemAsync(string itemId, string[]? categories = null, object? labels = null, string? comment = null, CancellationToken ct = default)
  {
    try
    {
      await _client.InsertItemAsync(new Item
      {
        ItemId = itemId,
        Categories = categories ?? Array.Empty<string>(),
        Labels = labels,
        Comment = comment ?? "",
        TimeStamp = DateTime.UtcNow
      });

      _logger.LogInformation("Inserted item {ItemId} into Gorse", itemId);
      return DomainResult.Success;
    }
    catch (GorseException ex) { return HandleException(ex, "InsertItem"); }
    catch (Exception ex) { return HandleUnexpected(ex, "InsertItem"); }
  }

  public async Task<Domain.Common.Results.Result<Domain.Common.Results.Success>> InsertItemsAsync(IEnumerable<RecommendationItem> items, CancellationToken ct = default)
  {
    try
    {
      var gorseItems = items.Select(i => new Item
      {
        ItemId = i.ItemId,
        Categories = i.Categories ?? Array.Empty<string>(),
        Labels = i.Labels,
        Comment = i.Comment ?? "",
        TimeStamp = DateTime.UtcNow
      }).ToList();

      await _client.InsertItemsAsync(gorseItems);

      _logger.LogInformation("Inserted {Count} items into Gorse", gorseItems.Count);
      return DomainResult.Success;
    }
    catch (GorseException ex) { return HandleException(ex, "InsertItems"); }
    catch (Exception ex) { return HandleUnexpected(ex, "InsertItems"); }
  }

  public async Task<Domain.Common.Results.Result<RecommendationItem>> GetItemAsync(string itemId, CancellationToken ct = default)
  {
    try
    {
      var item = await _client.GetItemAsync(itemId);
      return new RecommendationItem(item.ItemId, item.Categories, item.Labels, item.Comment);
    }
    catch (GorseException ex) { return HandleException(ex, "GetItem"); }
    catch (Exception ex) { return HandleUnexpected(ex, "GetItem"); }
  }

  public async Task<Domain.Common.Results.Result<Domain.Common.Results.Deleted>> DeleteItemAsync(string itemId, CancellationToken ct = default)
  {
    try
    {
      await _client.DeleteItemAsync(itemId);
      _logger.LogInformation("Deleted item {ItemId} from Gorse", itemId);
      return DomainResult.Deleted;
    }
    catch (GorseException ex) { return HandleException(ex, "DeleteItem"); }
    catch (Exception ex) { return HandleUnexpected(ex, "DeleteItem"); }
  }

  // ─── Feedback ────────────────────────────────

  public async Task<Domain.Common.Results.Result<Domain.Common.Results.Success>> InsertFeedbackAsync(IEnumerable<RecommendationFeedback> feedbacks, CancellationToken ct = default)
  {
    try
    {
      var gorseFeedback = feedbacks.Select(f => new Feedback
      {
        FeedbackType = f.FeedbackType,
        UserId = f.UserId,
        ItemId = f.ItemId,
        Timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ")
      }).ToArray();

      await _client.InsertFeedbackAsync(gorseFeedback);

      _logger.LogInformation("Inserted {Count} feedback entries into Gorse", gorseFeedback.Length);
      return DomainResult.Success;
    }
    catch (GorseException ex) { return HandleException(ex, "InsertFeedback"); }
    catch (Exception ex) { return HandleUnexpected(ex, "InsertFeedback"); }
  }

  // ─── Recommendations ─────────────────────────

  public async Task<Domain.Common.Results.Result<List<ScoredItem>>> GetRecommendationsAsync(string userId, CancellationToken ct = default)
  {
    try
    {
      var result = await _client.GetRecommendAsync(userId);

      if (result is not { Length: > 0 })
        return new List<ScoredItem>();

      // v0.5.0 returns List<string> (item IDs only, no scores)
      var scored = result
        .Select((id, index) => new ScoredItem(id, 1.0 - (index * 0.01)))
        .ToList();

      return scored;
    }
    catch (GorseException ex) { return HandleException(ex, "GetRecommendations"); }
    catch (Exception ex) { return HandleUnexpected(ex, "GetRecommendations"); }
  }

  public async Task<Domain.Common.Results.Result<List<ScoredItem>>> GetUserNeighborsAsync(string userId, int count = 10, CancellationToken ct = default)
  {
    try
    {
      var result = await _client.GetUserNeighborsAsync(userId, count);

      var scored = result
        .Select(r => new ScoredItem(r.Id, r.Score))
        .ToList();

      return scored;
    }
    catch (GorseException ex) { return HandleException(ex, "GetUserNeighbors"); }
    catch (Exception ex) { return HandleUnexpected(ex, "GetUserNeighbors"); }
  }

  // ─── Error Handling ──────────────────────────

  private DomainError HandleException(GorseException ex, string operation)
  {
    _logger.LogError(ex, "Gorse {Operation} failed. StatusCode: {StatusCode}", operation, ex.StatusCode);
    return DomainError.Failure($"Recommendation.{operation}Failed", ex.Message ?? $"Gorse {operation} request failed with status {ex.StatusCode}.");
  }

  private DomainError HandleUnexpected(Exception ex, string operation)
  {
    _logger.LogError(ex, "Unexpected error during Gorse {Operation}", operation);
    return DomainError.Unexpected($"Recommendation.{operation}Error", ex.Message);
  }
}
