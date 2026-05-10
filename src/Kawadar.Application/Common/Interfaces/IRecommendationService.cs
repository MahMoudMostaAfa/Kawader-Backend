using Kawadar.Application.Common.Models;
using Kawadar.Domain.Common.Results;

namespace Kawadar.Application.Common.Interfaces;

/// <summary>
/// Abstraction over the Gorse recommendation engine.
/// Provides methods for managing users, items, feedback (interactions),
/// and retrieving personalized recommendations.
/// </summary>
public interface IRecommendationService
{
  // ─── Users ───────────────────────────────────

  /// <summary>Insert a single user into the recommendation engine.</summary>
  Task<Result<Success>> InsertUserAsync(Guid userId, object? labels = null, string? comment = null, CancellationToken ct = default);

  /// <summary>Insert multiple users into the recommendation engine.</summary>
  Task<Result<Success>> InsertUsersAsync(IEnumerable<RecommendationUser> users, CancellationToken ct = default);

  /// <summary>Get a user by ID.</summary>
  Task<Result<RecommendationUser>> GetUserAsync(Guid userId, CancellationToken ct = default);

  /// <summary>Delete a user by ID.</summary>
  Task<Result<Deleted>> DeleteUserAsync(Guid userId, CancellationToken ct = default);

  // ─── Items ───────────────────────────────────

  /// <summary>Insert a single item into the recommendation engine.</summary>
  Task<Result<Success>> InsertItemAsync(string itemId, string[]? categories = null, object? labels = null, string? comment = null, CancellationToken ct = default);

  /// <summary>Insert multiple items into the recommendation engine.</summary>
  Task<Result<Success>> InsertItemsAsync(IEnumerable<RecommendationItem> items, CancellationToken ct = default);

  /// <summary>Get an item by ID.</summary>
  Task<Result<RecommendationItem>> GetItemAsync(string itemId, CancellationToken ct = default);

  /// <summary>Delete an item by ID.</summary>
  Task<Result<Deleted>> DeleteItemAsync(string itemId, CancellationToken ct = default);

  // ─── Feedback ────────────────────────────────

  /// <summary>
  /// Insert feedback (user-item interactions) into the recommendation engine.
  /// Feedback types: e.g. "like", "view", "click", "star", "purchase".
  /// </summary>
  Task<Result<Success>> InsertFeedbackAsync(IEnumerable<RecommendationFeedback> feedbacks, CancellationToken ct = default);

  // ─── Recommendations ─────────────────────────

  /// <summary>
  /// Get personalized item recommendations for a user.
  /// Returns a paginated list of job IDs ordered by relevance.
  /// </summary>
  Task<Result<PaginatedList<Guid>>> GetRecommendationsAsync(Guid userId, int pageNumber, int pageSize, CancellationToken ct = default);

  /// <summary>
  /// Get similar users (neighbors) for a given user.
  /// </summary>
  Task<Result<List<ScoredItem>>> GetUserNeighborsAsync(Guid userId, int count = 10, CancellationToken ct = default);
}


// ─── DTOs (kept in Application layer, no Gorse dependency) ───

/// <summary>A user in the recommendation engine.</summary>
public record RecommendationUser(Guid UserId, object? Labels = null, string? Comment = null);

/// <summary>An item in the recommendation engine.</summary>
public record RecommendationItem(string ItemId, string[]? Categories = null, object? Labels = null, string? Comment = null);

/// <summary>A user-item interaction.</summary>
public record RecommendationFeedback(string FeedbackType, Guid UserId, string ItemId);

/// <summary>A scored result (item or user) returned from the recommendation engine.</summary>
public record ScoredItem(Guid Id, double Score);
