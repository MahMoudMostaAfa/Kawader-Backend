
using Kawadar.Application.Common.Interfaces;
using Kawadar.Application.Common.Interfaces.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Kawadar.Api.Controllers.V1;


[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/test")]
public class TestController : ApiController
{
  private readonly IRecommendationService _recommendation;
  private readonly IUnitOfWork _unitOfWork;
  private readonly IWalletRepository _walletRepository;

  public TestController(IRecommendationService recommendation, IUnitOfWork unitOfWork, IWalletRepository walletRepository)
  {
    _unitOfWork = unitOfWork;
    _walletRepository = walletRepository;

    _recommendation = recommendation;
  }

  // ──────────────────────────────────────────────
  // Users
  // ──────────────────────────────────────────────

  /// <summary>
  /// Insert a single user into Gorse.
  /// </summary>
  [HttpPost("gorse/users")]
  public async Task<IActionResult> InsertGorseUser([FromBody] InsertUserRequest request, CancellationToken ct)
  {
    var result = await _recommendation.InsertUserAsync(request.UserId, request.Labels, request.Comment, ct);

    return result.IsSuccess
      ? Ok(new { message = $"User '{request.UserId}' inserted." })
      : Problem(detail: result.TopError.Description, title: result.TopError.Code, statusCode: StatusCodes.Status502BadGateway);
  }

  /// <summary>
  /// Get a user by ID from Gorse.
  /// </summary>
  [HttpGet("gorse/users/{userId}")]
  public async Task<IActionResult> GetGorseUser(Guid userId, CancellationToken ct)
  {
    var result = await _recommendation.GetUserAsync(userId, ct);

    return result.IsSuccess
      ? Ok(result.Value)
      : Problem(detail: result.TopError.Description, title: result.TopError.Code, statusCode: StatusCodes.Status502BadGateway);
  }

  /// <summary>
  /// Delete a user from Gorse.
  /// </summary>
  [HttpDelete("gorse/users/{userId}")]
  public async Task<IActionResult> DeleteGorseUser(Guid userId, CancellationToken ct)
  {
    var result = await _recommendation.DeleteUserAsync(userId, ct);

    return result.IsSuccess
      ? Ok(new { message = $"User '{userId}' deleted." })
      : Problem(detail: result.TopError.Description, title: result.TopError.Code, statusCode: StatusCodes.Status502BadGateway);
  }

  // ──────────────────────────────────────────────
  // Items
  // ──────────────────────────────────────────────

  /// <summary>
  /// Insert a single item into Gorse.
  /// </summary>
  [HttpPost("gorse/items")]
  public async Task<IActionResult> InsertGorseItem([FromBody] InsertItemRequest request, CancellationToken ct)
  {
    var result = await _recommendation.InsertItemAsync(request.ItemId, request.Categories, request.Labels, request.Comment, ct);

    return result.IsSuccess
      ? Ok(new { message = $"Item '{request.ItemId}' inserted." })
      : Problem(detail: result.TopError.Description, title: result.TopError.Code, statusCode: StatusCodes.Status502BadGateway);
  }

  /// <summary>
  /// Get an item by ID from Gorse.
  /// </summary>
  [HttpGet("gorse/items/{itemId}")]
  public async Task<IActionResult> GetGorseItem(string itemId, CancellationToken ct)
  {
    var result = await _recommendation.GetItemAsync(itemId, ct);

    return result.IsSuccess
      ? Ok(result.Value)
      : Problem(detail: result.TopError.Description, title: result.TopError.Code, statusCode: StatusCodes.Status502BadGateway);
  }

  /// <summary>
  /// Delete an item from Gorse.
  /// </summary>
  [HttpDelete("gorse/items/{itemId}")]
  public async Task<IActionResult> DeleteGorseItem(string itemId, CancellationToken ct)
  {
    var result = await _recommendation.DeleteItemAsync(itemId, ct);

    return result.IsSuccess
      ? Ok(new { message = $"Item '{itemId}' deleted." })
      : Problem(detail: result.TopError.Description, title: result.TopError.Code, statusCode: StatusCodes.Status502BadGateway);
  }

  // ──────────────────────────────────────────────
  // Feedback (user-item interactions)
  // ──────────────────────────────────────────────

  /// <summary>
  /// Insert feedback (interactions) into Gorse.
  /// Feedback types examples: "star", "like", "view", "click", "purchase".
  /// </summary>
  [HttpPost("gorse/feedback")]
  public async Task<IActionResult> InsertGorseFeedback([FromBody] InsertFeedbackRequest[] feedbacks, CancellationToken ct)
  {
    var mapped = feedbacks.Select(f => new RecommendationFeedback(f.FeedbackType, f.UserId, f.ItemId));
    var result = await _recommendation.InsertFeedbackAsync(mapped, ct);

    return result.IsSuccess
      ? Ok(new { message = $"{feedbacks.Length} feedback entries inserted." })
      : Problem(detail: result.TopError.Description, title: result.TopError.Code, statusCode: StatusCodes.Status502BadGateway);
  }

  // ──────────────────────────────────────────────
  // Recommendations & Neighbors
  // ──────────────────────────────────────────────

  /// <summary>
  /// Get personalized item recommendations for a user.
  /// Note: Recommendations are generated after the model trains (fit_period).
  /// </summary>
  [HttpGet("gorse/recommend/{userId}")]
  public async Task<IActionResult> GetGorseRecommendations(Guid userId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, CancellationToken ct = default)
  {
    var result = await _recommendation.GetRecommendationsAsync(userId, pageNumber, pageSize, ct);

    return result.IsSuccess
      ? Ok(result.Value)
      : Problem(detail: result.TopError.Description, title: result.TopError.Code, statusCode: StatusCodes.Status502BadGateway);
  }

  /// <summary>
  /// Get similar users (neighbors) for a given user.
  /// </summary>
  [HttpGet("gorse/neighbors/{userId}")]
  public async Task<IActionResult> GetGorseUserNeighbors(Guid userId, [FromQuery] int n = 10, CancellationToken ct = default)
  {
    var result = await _recommendation.GetUserNeighborsAsync(userId, n, ct);

    return result.IsSuccess
      ? Ok(result.Value)
      : Problem(detail: result.TopError.Description, title: result.TopError.Code, statusCode: StatusCodes.Status502BadGateway);
  }

  // ──────────────────────────────────────────────
  // Seed: Populate sample data for quick testing
  // ──────────────────────────────────────────────

  /// <summary>
  /// Seeds Gorse with sample users, items, and feedback for testing.
  /// Creates 3 users, 5 items (job postings), and various interactions.
  /// </summary>
  [HttpPost("gorse/seed")]
  public async Task<IActionResult> SeedGorseData(CancellationToken ct)
  {
    // 1. Insert sample users
    var user1Id = Guid.NewGuid();
    var user2Id = Guid.NewGuid();
    var user3Id = Guid.NewGuid();

    var users = new[]
    {
      new RecommendationUser(user1Id, new { skills = new[] { "dotnet", "csharp", "sql" }, level = "senior" }, "Ahmed - Senior .NET Developer"),
      new RecommendationUser(user2Id, new { skills = new[] { "react", "typescript", "nodejs" }, level = "mid" }, "Sara - Mid Frontend Developer"),
      new RecommendationUser(user3Id, new { skills = new[] { "dotnet", "angular", "sql" }, level = "junior" }, "Omar - Junior Full-Stack Developer"),
    };
    var usersResult = await _recommendation.InsertUsersAsync(users, ct);
    if (usersResult.IsError)
      return Problem(detail: usersResult.TopError.Description, title: "Seed failed at users");

    // 2. Insert sample items (job postings / gigs)
    var items = new[]
    {
      new RecommendationItem("job-1", new[] { "backend", "dotnet" }, new { budget = 500, duration = "1 month" }, "Build REST API with ASP.NET Core"),
      new RecommendationItem("job-2", new[] { "frontend", "react" }, new { budget = 300, duration = "2 weeks" }, "React Dashboard UI"),
      new RecommendationItem("job-3", new[] { "backend", "dotnet" }, new { budget = 800, duration = "2 months" }, "Microservices Architecture with .NET"),
      new RecommendationItem("job-4", new[] { "fullstack", "angular", "dotnet" }, new { budget = 600, duration = "1 month" }, "Full-Stack App with Angular + .NET"),
      new RecommendationItem("job-5", new[] { "frontend", "react", "typescript" }, new { budget = 400, duration = "3 weeks" }, "TypeScript Component Library"),
    };
    var itemsResult = await _recommendation.InsertItemsAsync(items, ct);
    if (itemsResult.IsError)
      return Problem(detail: itemsResult.TopError.Description, title: "Seed failed at items");

    // 3. Insert feedback (interactions)
    var feedbacks = new[]
    {
      // user-1 (dotnet dev) likes dotnet jobs
      new RecommendationFeedback("like", user1Id, "job-1"),
      new RecommendationFeedback("like", user1Id, "job-3"),
      new RecommendationFeedback("view", user1Id, "job-4"),

      // user-2 (react dev) likes frontend jobs
      new RecommendationFeedback("like", user2Id, "job-2"),
      new RecommendationFeedback("like", user2Id, "job-5"),
      new RecommendationFeedback("view", user2Id, "job-1"),

      // user-3 (full-stack) likes mixed jobs
      new RecommendationFeedback("like", user3Id, "job-4"),
      new RecommendationFeedback("like", user3Id, "job-1"),
      new RecommendationFeedback("view", user3Id, "job-3"),
      new RecommendationFeedback("view", user3Id, "job-2"),
    };
    var feedbackResult = await _recommendation.InsertFeedbackAsync(feedbacks, ct);
    if (feedbackResult.IsError)
      return Problem(detail: feedbackResult.TopError.Description, title: "Seed failed at feedback");

    return Ok(new
    {
      Message = "Gorse seeded successfully!",
      UsersInserted = users.Length,
      ItemsInserted = items.Length,
      FeedbackInserted = feedbacks.Length,
      UserIds = new[] { user1Id, user2Id, user3Id },
      Hint = $"Wait ~1-2 minutes for the model to train, then call GET /gorse/recommend/{user1Id} to see recommendations."
    });
  }



  [HttpPost("wallet/insert-transaction")]

  public async Task<IActionResult> InsertWalletTransaction([FromBody] InsertWalletTransactionRequest request, CancellationToken ct)
  {
    var walletResult = await _walletRepository.GetByUserIdAsync(request.UserId);
    if (walletResult.IsError) return Problem(walletResult.Errors);

    var wallet = walletResult.Value;
    wallet.AddTransaction(request.Amount, Domain.WalletAndPayments.Enums.TransactionType.Deposit, Domain.WalletAndPayments.Enums.WalletTransactionReferenceType.Manual, Guid.CreateVersion7(), null, Domain.WalletAndPayments.Enums.WalletTransactionStatus.Completed);
    await _unitOfWork.SaveChangesAsync(ct);


    return Ok();
  }
}

// ──────────────────────────────────────────────
// Request DTOs
// ──────────────────────────────────────────────

public record InsertWalletTransactionRequest(Guid UserId, decimal Amount);

public record InsertUserRequest(Guid UserId, object? Labels = null, string? Comment = null);
public record InsertItemRequest(string ItemId, string[]? Categories = null, object? Labels = null, string? Comment = null);
public record InsertFeedbackRequest(string FeedbackType, Guid UserId, string ItemId);