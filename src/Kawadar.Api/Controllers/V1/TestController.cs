
using Kawadar.Application.Common.Interfaces;
using Kawadar.Application.Common.Interfaces.Auth;
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
  private readonly IIdentityService _identityService;
  private readonly IUsersRepository _usersRepository;
  private readonly ISkillRepository _skillRepository;
  private readonly ISpecilizationRepository _specilizationRepository;

  public TestController(
    IRecommendationService recommendation,
    IUnitOfWork unitOfWork,
    IWalletRepository walletRepository,
    IIdentityService identityService,
    IUsersRepository usersRepository,
    ISkillRepository skillRepository,
    ISpecilizationRepository specilizationRepository)
  {
    _unitOfWork = unitOfWork;
    _walletRepository = walletRepository;
    _recommendation = recommendation;
    _identityService = identityService;
    _usersRepository = usersRepository;
    _skillRepository = skillRepository;
    _specilizationRepository = specilizationRepository;
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
  /// Get raw recommendations from Gorse (returns item IDs as strings).
  /// Useful for testing with non-GUID item IDs like "job-web-api".
  /// </summary>
  [HttpGet("gorse/recommend-raw/{userId}")]
  public async Task<IActionResult> GetGorseRecommendationsRaw(Guid userId, CancellationToken ct = default)
  {
    try
    {
      var gorseClient = HttpContext.RequestServices.GetRequiredService<IRecommendationService>();
      // Call the underlying Gorse API directly via the service
      var result = await _recommendation.GetRecommendationsRawAsync(userId, ct);

      return result.IsSuccess
        ? Ok(new { UserId = userId, Recommendations = result.Value })
        : Problem(detail: result.TopError.Description, title: result.TopError.Code, statusCode: StatusCodes.Status502BadGateway);
    }
    catch (Exception ex)
    {
      return Problem(detail: ex.Message, title: "Recommendation error");
    }
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

  /// <summary>
  /// Reset the entire Gorse recommendation engine.
  /// Deletes all users, items, and feedback. Use with caution.
  /// </summary>
  [HttpDelete("gorse/reset")]
  public async Task<IActionResult> ResetGorse(CancellationToken ct)
  {
    var result = await _recommendation.ResetAsync(ct);

    return result.IsSuccess
      ? Ok(new { message = "Gorse recommendation engine reset complete." })
      : Problem(detail: result.TopError.Description, title: result.TopError.Code, statusCode: StatusCodes.Status502BadGateway);
  }

  /// <summary>
  /// Insert two specific users (by email) from the database into Gorse with full labels.
  /// </summary>
  [HttpPost("gorse/seed-real-users")]
  public async Task<IActionResult> SeedRealUsersToGorse(CancellationToken ct)
  {
    var emails = new[] { "mahmoud2030m2@gmail.com", "mahmoud2030m@gmail.com" };
    var insertedUsers = new List<object>();

    foreach (var email in emails)
    {
      // 1. Get identity user by email
      var identityResult = await _identityService.GetUserByEmailAsync(email);
      if (identityResult.IsError)
        return Problem(detail: $"Identity user not found for '{email}': {identityResult.TopError.Description}", title: "User lookup failed");

      var identityUser = identityResult.Value;

      // 2. Get UserProfile by identity userId
      var profileResult = await _usersRepository.GetUserProfileByUserIdAsync(identityUser.Id);
      if (profileResult.IsError)
        return Problem(detail: $"UserProfile not found for '{email}': {profileResult.TopError.Description}", title: "Profile lookup failed");

      var profile = profileResult.Value;

      // 3. Get freelancer skills
      var skillNames = await _skillRepository.GetFreelancerSkillsByUserProfileId(profile.Id);

      // 4. Build labels: skills + specialization + experience + profileType
      var labels = skillNames
        .Select(s => s.ToLower())
        .Concat(new[] { profile.ExperienceYear.ToString().ToLower(), profile.ProfileType.ToString().ToLower() })
        .ToList();

      if (profile.SpecializationId.HasValue)
      {
        var specResult = await _specilizationRepository.GetById(profile.SpecializationId.Value);
        if (!specResult.IsError)
          labels.Add(specResult.Value.Name.ToLower());
      }

      // 5. Insert into Gorse
      var gorseResult = await _recommendation.InsertUserAsync(
        profile.Id,
        labels: labels.ToArray(),
        comment: profile.FullName,
        ct: ct);

      if (gorseResult.IsError)
        return Problem(detail: $"Gorse insert failed for '{email}': {gorseResult.TopError.Description}", title: "Gorse insert failed");

      insertedUsers.Add(new
      {
        Email = email,
        UserProfileId = profile.Id,
        Labels = labels,
        Name = profile.FullName
      });
    }

    return Ok(new
    {
      Message = $"Successfully inserted {insertedUsers.Count} real users into Gorse.",
      Users = insertedUsers
    });
  }

  /// <summary>
  /// Insert custom job items into Gorse and create feedback from the two real users.
  /// Call seed-real-users first to ensure the users exist in Gorse.
  /// </summary>
  [HttpPost("gorse/seed-jobs-and-feedback")]
  public async Task<IActionResult> SeedJobsAndFeedback(CancellationToken ct)
  {
    // 1. Resolve the two real users to get their UserProfile IDs
    var emails = new[] { "mahmoud2030m2@gmail.com", "mahmoud2030m@gmail.com" };
    var userProfileIds = new List<Guid>();

    foreach (var email in emails)
    {
      var identityResult = await _identityService.GetUserByEmailAsync(email);
      if (identityResult.IsError)
        return Problem(detail: $"User '{email}' not found. Run seed-real-users first.", title: "User lookup failed");

      var profileResult = await _usersRepository.GetUserProfileByUserIdAsync(identityResult.Value.Id);
      if (profileResult.IsError)
        return Problem(detail: $"Profile not found for '{email}'.", title: "Profile lookup failed");

      userProfileIds.Add(profileResult.Value.Id);
    }

    var user1 = userProfileIds[0]; // mahmoud2030m2
    var user2 = userProfileIds[1]; // mahmoud2030m

    // 2. Generate GUID IDs for each job (just like CreateJobCommandHandler does)
    var jobIds = new Dictionary<string, Guid>
    {
      ["web-api"] = Guid.NewGuid(),
      ["react-dashboard"] = Guid.NewGuid(),
      ["mobile-app"] = Guid.NewGuid(),
      ["ecommerce"] = Guid.NewGuid(),
      ["data-analysis"] = Guid.NewGuid(),
      ["wordpress-site"] = Guid.NewGuid(),
      ["dotnet-microservices"] = Guid.NewGuid(),
      ["ui-design"] = Guid.NewGuid(),
      ["nodejs-backend"] = Guid.NewGuid(),
      ["devops-cicd"] = Guid.NewGuid(),
    };

    // 3. Insert job items with GUID IDs, realistic categories and labels
    var jobs = new[]
    {
      new RecommendationItem(jobIds["web-api"].ToString(),
        new[] { "Web Development" },
        new[] { "asp.net core", "c#", "sql server", "rest api", "fixed", "entrylevel" },
        "Build a REST API with ASP.NET Core"),

      new RecommendationItem(jobIds["react-dashboard"].ToString(),
        new[] { "Web Development" },
        new[] { "react", "typescript", "css", "frontend", "hourly", "midlevel" },
        "React Admin Dashboard UI"),

      new RecommendationItem(jobIds["mobile-app"].ToString(),
        new[] { "Mobile Development" },
        new[] { "flutter", "dart", "firebase", "mobile", "fixed", "midlevel" },
        "Cross-platform Mobile App with Flutter"),

      new RecommendationItem(jobIds["ecommerce"].ToString(),
        new[] { "Web Development" },
        new[] { "asp.net core", "angular", "sql server", "fullstack", "fixed", "seniorlevel" },
        "Full-Stack E-Commerce Platform"),

      new RecommendationItem(jobIds["data-analysis"].ToString(),
        new[] { "Data Science" },
        new[] { "python", "pandas", "sql", "data analysis", "hourly", "entrylevel" },
        "Data Analysis & Reporting Dashboard"),

      new RecommendationItem(jobIds["wordpress-site"].ToString(),
        new[] { "Web Development" },
        new[] { "wordpress", "php", "css", "seo", "fixed", "entrylevel" },
        "WordPress Business Website"),

      new RecommendationItem(jobIds["dotnet-microservices"].ToString(),
        new[] { "Web Development" },
        new[] { "asp.net core", "c#", "docker", "rabbitmq", "microservices", "fixed", "seniorlevel" },
        "Microservices Architecture with .NET"),

      new RecommendationItem(jobIds["ui-design"].ToString(),
        new[] { "Graphic Design" },
        new[] { "figma", "ui/ux", "prototyping", "design system", "fixed", "midlevel" },
        "Mobile App UI/UX Design"),

      new RecommendationItem(jobIds["nodejs-backend"].ToString(),
        new[] { "Web Development" },
        new[] { "nodejs", "express", "mongodb", "rest api", "hourly", "midlevel" },
        "Node.js Backend for SaaS Platform"),

      new RecommendationItem(jobIds["devops-cicd"].ToString(),
        new[] { "DevOps" },
        new[] { "docker", "kubernetes", "github actions", "ci/cd", "linux", "hourly", "seniorlevel" },
        "CI/CD Pipeline Setup & DevOps"),
    };

    var itemsResult = await _recommendation.InsertItemsAsync(jobs, ct);
    if (itemsResult.IsError)
      return Problem(detail: itemsResult.TopError.Description, title: "Failed to insert job items");

    // 4. Insert feedback — simulate realistic interactions
    var feedbacks = new[]
    {
      // User 1 — interested in .NET / backend jobs
      new RecommendationFeedback("star", user1, jobIds["web-api"].ToString()),
      new RecommendationFeedback("star", user1, jobIds["dotnet-microservices"].ToString()),
      new RecommendationFeedback("like", user1, jobIds["ecommerce"].ToString()),
      new RecommendationFeedback("view", user1, jobIds["react-dashboard"].ToString()),
      new RecommendationFeedback("view", user1, jobIds["nodejs-backend"].ToString()),
      new RecommendationFeedback("view", user1, jobIds["devops-cicd"].ToString()),

      // User 2 — interested in frontend / design jobs
      new RecommendationFeedback("star", user2, jobIds["react-dashboard"].ToString()),
      new RecommendationFeedback("star", user2, jobIds["ui-design"].ToString()),
      new RecommendationFeedback("like", user2, jobIds["wordpress-site"].ToString()),
      new RecommendationFeedback("like", user2, jobIds["nodejs-backend"].ToString()),
      new RecommendationFeedback("view", user2, jobIds["web-api"].ToString()),
      new RecommendationFeedback("view", user2, jobIds["mobile-app"].ToString()),
    };

    var feedbackResult = await _recommendation.InsertFeedbackAsync(feedbacks, ct);
    if (feedbackResult.IsError)
      return Problem(detail: feedbackResult.TopError.Description, title: "Failed to insert feedback");

    return Ok(new
    {
      Message = "Jobs and feedback seeded successfully!",
      JobsInserted = jobs.Length,
      FeedbackInserted = feedbacks.Length,
      JobIds = jobIds,
      User1 = new { Id = user1, Email = emails[0], Pattern = "Backend/.NET focused" },
      User2 = new { Id = user2, Email = emails[1], Pattern = "Frontend/Design focused" },
      Hint = $"Wait ~2 minutes for training, then call GET /api/v1/test/gorse/recommend/{user1}"
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