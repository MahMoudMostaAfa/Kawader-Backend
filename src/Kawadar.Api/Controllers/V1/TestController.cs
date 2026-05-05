
using Gorse.NET.Models;
using GorseClient = Gorse.NET.Gorse;
using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;

namespace Kawadar.Api.Controllers.V1;


[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/test")]
public class TestController : ApiController
{

  private readonly AppDbContext appDbContext;
  private readonly IIdentityService _identityService;
  private readonly ILogger<TestController> logger;
  private readonly IConfiguration configuration;
  public TestController(AppDbContext appDbContext, ILogger<TestController> logger, IIdentityService identityService, IConfiguration configuration)
  {
    this.appDbContext = appDbContext;
    this.logger = logger;
    _identityService = identityService;
    this.configuration = configuration;
  }

  // ──────────────────────────────────────────────
  // Helper: Creates a configured Gorse client
  // ──────────────────────────────────────────────

  private GorseClient? CreateGorseClient(out string? error)
  {
    var gorseSection = configuration.GetSection("Gorse");
    var baseUri = gorseSection["BaseUri"];
    var apiKey = gorseSection["ApiKey"];

    if (string.IsNullOrWhiteSpace(baseUri) || string.IsNullOrWhiteSpace(apiKey))
    {
      error = "Gorse configuration (BaseUri / ApiKey) is missing.";
      return null;
    }

    error = null;
    return new GorseClient(baseUri, apiKey);
  }

  private IActionResult HandleGorseException(Gorse.NET.Utilities.GorseException ex)
  {
    var statusCodeValue = ex.StatusCode.ToString();
    var detail = string.IsNullOrWhiteSpace(ex.Message)
      ? $"Gorse request failed. StatusCode: {statusCodeValue}."
      : ex.Message;

    logger.LogError(ex, "Gorse request failed. StatusCode: {StatusCode}", statusCodeValue);

    return Problem(
      title: "Gorse request failed",
      detail: detail,
      statusCode: StatusCodes.Status502BadGateway);
  }

  // ──────────────────────────────────────────────
  // Users
  // ──────────────────────────────────────────────

  /// <summary>
  /// Insert a single user into Gorse.
  /// </summary>
  [HttpPost("gorse/users")]
  public async Task<IActionResult> InsertGorseUser([FromBody] InsertUserRequest request)
  {
    var gorse = CreateGorseClient(out var error);
    if (gorse is null) return BadRequest(error);

    try
    {
      var result = await gorse.InsertUserAsync(new User
      {
        UserId = request.UserId,
        Labels = request.Labels,
        Comment = request.Comment ?? ""
      });
      return Ok(result);
    }
    catch (Gorse.NET.Utilities.GorseException ex) { return HandleGorseException(ex); }
  }

  /// <summary>
  /// Get a user by ID from Gorse.
  /// </summary>
  [HttpGet("gorse/users/{userId}")]
  public async Task<IActionResult> GetGorseUser(string userId)
  {
    var gorse = CreateGorseClient(out var error);
    if (gorse is null) return BadRequest(error);

    try
    {
      var user = await gorse.GetUserAsync(userId);
      return Ok(user);
    }
    catch (Gorse.NET.Utilities.GorseException ex) { return HandleGorseException(ex); }
  }

  /// <summary>
  /// List all users in Gorse.
  /// </summary>
  [HttpGet("gorse/users")]
  public async Task<IActionResult> GetGorseUsers([FromQuery] int n = 20, [FromQuery] string cursor = "")
  {
    var gorse = CreateGorseClient(out var error);
    if (gorse is null) return BadRequest(error);

    try
    {
      var result = await gorse.GetUsersAsync(n, cursor);
      return Ok(result);
    }
    catch (Gorse.NET.Utilities.GorseException ex) { return HandleGorseException(ex); }
  }

  /// <summary>
  /// Delete a user from Gorse.
  /// </summary>
  [HttpDelete("gorse/users/{userId}")]
  public async Task<IActionResult> DeleteGorseUser(string userId)
  {
    var gorse = CreateGorseClient(out var error);
    if (gorse is null) return BadRequest(error);

    try
    {
      var result = await gorse.DeleteUserAsync(userId);
      return Ok(result);
    }
    catch (Gorse.NET.Utilities.GorseException ex) { return HandleGorseException(ex); }
  }

  // ──────────────────────────────────────────────
  // Items
  // ──────────────────────────────────────────────

  /// <summary>
  /// Insert a single item into Gorse.
  /// </summary>
  [HttpPost("gorse/items")]
  public async Task<IActionResult> InsertGorseItem([FromBody] InsertItemRequest request)
  {
    var gorse = CreateGorseClient(out var error);
    if (gorse is null) return BadRequest(error);

    try
    {
      var result = await gorse.InsertItemAsync(new Item
      {
        ItemId = request.ItemId,
        Categories = request.Categories ?? Array.Empty<string>(),
        Labels = request.Labels,
        Comment = request.Comment ?? "",
        IsHidden = request.IsHidden,
        TimeStamp = DateTime.UtcNow
      });
      return Ok(result);
    }
    catch (Gorse.NET.Utilities.GorseException ex) { return HandleGorseException(ex); }
  }

  /// <summary>
  /// Get an item by ID from Gorse.
  /// </summary>
  [HttpGet("gorse/items/{itemId}")]
  public async Task<IActionResult> GetGorseItem(string itemId)
  {
    var gorse = CreateGorseClient(out var error);
    if (gorse is null) return BadRequest(error);

    try
    {
      var item = await gorse.GetItemAsync(itemId);
      return Ok(item);
    }
    catch (Gorse.NET.Utilities.GorseException ex) { return HandleGorseException(ex); }
  }

  /// <summary>
  /// List all items in Gorse.
  /// </summary>
  [HttpGet("gorse/items")]
  public async Task<IActionResult> GetGorseItems([FromQuery] int n = 20, [FromQuery] string cursor = "")
  {
    var gorse = CreateGorseClient(out var error);
    if (gorse is null) return BadRequest(error);

    try
    {
      var result = await gorse.GetItemsAsync(n, cursor);
      return Ok(result);
    }
    catch (Gorse.NET.Utilities.GorseException ex) { return HandleGorseException(ex); }
  }

  /// <summary>
  /// Delete an item from Gorse.
  /// </summary>
  [HttpDelete("gorse/items/{itemId}")]
  public async Task<IActionResult> DeleteGorseItem(string itemId)
  {
    var gorse = CreateGorseClient(out var error);
    if (gorse is null) return BadRequest(error);

    try
    {
      var result = await gorse.DeleteItemAsync(itemId);
      return Ok(result);
    }
    catch (Gorse.NET.Utilities.GorseException ex) { return HandleGorseException(ex); }
  }

  // ──────────────────────────────────────────────
  // Feedback (user-item interactions)
  // ──────────────────────────────────────────────

  /// <summary>
  /// Insert feedback (interactions) into Gorse.
  /// Feedback types examples: "star", "like", "view", "click", "purchase".
  /// </summary>
  [HttpPost("gorse/feedback")]
  public async Task<IActionResult> InsertGorseFeedback([FromBody] InsertFeedbackRequest[] feedbacks)
  {
    var gorse = CreateGorseClient(out var error);
    if (gorse is null) return BadRequest(error);

    try
    {
      var mapped = feedbacks.Select(f => new Feedback
      {
        FeedbackType = f.FeedbackType,
        UserId = f.UserId,
        ItemId = f.ItemId,
        Timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ")
      }).ToArray();

      var result = await gorse.InsertFeedbackAsync(mapped);
      return Ok(result);
    }
    catch (Gorse.NET.Utilities.GorseException ex) { return HandleGorseException(ex); }
  }

  /// <summary>
  /// List all feedback in Gorse.
  /// </summary>
  [HttpGet("gorse/feedback")]
  public async Task<IActionResult> GetGorseFeedback([FromQuery] int n = 20, [FromQuery] string cursor = "")
  {
    var gorse = CreateGorseClient(out var error);
    if (gorse is null) return BadRequest(error);

    try
    {
      var result = await gorse.GetFeedbacksAsync(n, cursor);
      return Ok(result);
    }
    catch (Gorse.NET.Utilities.GorseException ex) { return HandleGorseException(ex); }
  }

  // ──────────────────────────────────────────────
  // Recommendations & Neighbors
  // ──────────────────────────────────────────────

  /// <summary>
  /// Get personalized item recommendations for a user.
  /// Note: Recommendations are generated after the model trains (fit_period).
  /// </summary>
  [HttpGet("gorse/recommend/{userId}")]
  public async Task<IActionResult> GetGorseRecommendations(string userId)
  {
    var gorse = CreateGorseClient(out var error);
    if (gorse is null) return BadRequest(error);

    try
    {
      var result = await gorse.GetRecommendAsync(userId);
      return Ok(result);
    }
    catch (Gorse.NET.Utilities.GorseException ex) { return HandleGorseException(ex); }
  }

  /// <summary>
  /// Get similar users (neighbors) for a given user.
  /// </summary>
  [HttpGet("gorse/neighbors/{userId}")]
  public async Task<IActionResult> GetGorseUserNeighbors(string userId, [FromQuery] int n = 10)
  {
    var gorse = CreateGorseClient(out var error);
    if (gorse is null) return BadRequest(error);

    try
    {
      var result = await gorse.GetUserNeighborsAsync(userId, n);
      return Ok(result);
    }
    catch (Gorse.NET.Utilities.GorseException ex) { return HandleGorseException(ex); }
  }

  // ──────────────────────────────────────────────
  // Seed: Populate sample data for quick testing
  // ──────────────────────────────────────────────

  /// <summary>
  /// Seeds Gorse with sample users, items, and feedback for testing.
  /// Creates 3 users, 5 items (job postings), and various interactions.
  /// </summary>
  [HttpPost("gorse/seed")]
  public async Task<IActionResult> SeedGorseData()
  {
    var gorse = CreateGorseClient(out var error);
    if (gorse is null) return BadRequest(error);

    try
    {
      // 1. Insert sample users
      var users = new List<User>
      {
        new() { UserId = "user-1", Labels = new { skills = new[] { "dotnet", "csharp", "sql" }, level = "senior" }, Comment = "Ahmed - Senior .NET Developer" },
        new() { UserId = "user-2", Labels = new { skills = new[] { "react", "typescript", "nodejs" }, level = "mid" }, Comment = "Sara - Mid Frontend Developer" },
        new() { UserId = "user-3", Labels = new { skills = new[] { "dotnet", "angular", "sql" }, level = "junior" }, Comment = "Omar - Junior Full-Stack Developer" },
      };
      var usersResult = await gorse.InsertUsersAsync(users);

      // 2. Insert sample items (job postings / gigs)
      var items = new List<Item>
      {
        new() { ItemId = "job-1", Categories = new[] { "backend", "dotnet" }, Labels = new { budget = 500, duration = "1 month" }, Comment = "Build REST API with ASP.NET Core", TimeStamp = DateTime.UtcNow },
        new() { ItemId = "job-2", Categories = new[] { "frontend", "react" }, Labels = new { budget = 300, duration = "2 weeks" }, Comment = "React Dashboard UI", TimeStamp = DateTime.UtcNow },
        new() { ItemId = "job-3", Categories = new[] { "backend", "dotnet" }, Labels = new { budget = 800, duration = "2 months" }, Comment = "Microservices Architecture with .NET", TimeStamp = DateTime.UtcNow },
        new() { ItemId = "job-4", Categories = new[] { "fullstack", "angular", "dotnet" }, Labels = new { budget = 600, duration = "1 month" }, Comment = "Full-Stack App with Angular + .NET", TimeStamp = DateTime.UtcNow },
        new() { ItemId = "job-5", Categories = new[] { "frontend", "react", "typescript" }, Labels = new { budget = 400, duration = "3 weeks" }, Comment = "TypeScript Component Library", TimeStamp = DateTime.UtcNow },
      };
      var itemsResult = await gorse.InsertItemsAsync(items);

      // 3. Insert feedback (interactions)
      var now = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ");
      var feedbacks = new[]
      {
        // user-1 (dotnet dev) likes dotnet jobs
        new Feedback { FeedbackType = "like", UserId = "user-1", ItemId = "job-1", Timestamp = now },
        new Feedback { FeedbackType = "like", UserId = "user-1", ItemId = "job-3", Timestamp = now },
        new Feedback { FeedbackType = "view", UserId = "user-1", ItemId = "job-4", Timestamp = now },

        // user-2 (react dev) likes frontend jobs
        new Feedback { FeedbackType = "like", UserId = "user-2", ItemId = "job-2", Timestamp = now },
        new Feedback { FeedbackType = "like", UserId = "user-2", ItemId = "job-5", Timestamp = now },
        new Feedback { FeedbackType = "view", UserId = "user-2", ItemId = "job-1", Timestamp = now },

        // user-3 (full-stack) likes mixed jobs
        new Feedback { FeedbackType = "like", UserId = "user-3", ItemId = "job-4", Timestamp = now },
        new Feedback { FeedbackType = "like", UserId = "user-3", ItemId = "job-1", Timestamp = now },
        new Feedback { FeedbackType = "view", UserId = "user-3", ItemId = "job-3", Timestamp = now },
        new Feedback { FeedbackType = "view", UserId = "user-3", ItemId = "job-2", Timestamp = now },
      };
      var feedbackResult = await gorse.InsertFeedbackAsync(feedbacks);

      return Ok(new
      {
        Message = "Gorse seeded successfully!",
        Users = usersResult,
        Items = itemsResult,
        Feedback = feedbackResult,
        Hint = "Wait ~1-2 minutes for the model to train, then call GET /gorse/recommend/user-1 to see recommendations."
      });
    }
    catch (Gorse.NET.Utilities.GorseException ex) { return HandleGorseException(ex); }
  }
}

// ──────────────────────────────────────────────
// Request DTOs
// ──────────────────────────────────────────────

public record InsertUserRequest(string UserId, object? Labels = null, string? Comment = null);
public record InsertItemRequest(string ItemId, string[]? Categories = null, object? Labels = null, string? Comment = null, bool IsHidden = false);
public record InsertFeedbackRequest(string FeedbackType, string UserId, string ItemId);