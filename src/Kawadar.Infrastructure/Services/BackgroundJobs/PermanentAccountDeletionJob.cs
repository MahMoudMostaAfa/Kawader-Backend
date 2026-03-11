using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Application.Common.Interfaces.Repositories;
using Microsoft.Extensions.Logging;

namespace Kawadar.Infrastructure.Services.BackgroundJobs;

public class PermanentAccountDeletionJob
{
  private readonly IUsersRepository _usersRepository;
  private readonly IIdentityService _identityService;
  private readonly IUnitOfWork _unitOfWork;
  private readonly ILogger<PermanentAccountDeletionJob> _logger;

  public PermanentAccountDeletionJob(
    IUsersRepository usersRepository,
    IIdentityService identityService,
    IUnitOfWork unitOfWork,
    ILogger<PermanentAccountDeletionJob> logger)
  {
    _usersRepository = usersRepository;
    _identityService = identityService;
    _unitOfWork = unitOfWork;
    _logger = logger;
  }

  public async Task ExecuteAsync(string userId, CancellationToken cancellationToken)
  {
    _logger.LogInformation("Starting permanent account deletion for user {UserId}", userId);

    var profileResult = await _usersRepository.GetUserProfileByUserIdAsync(userId);
    if (profileResult.IsError)
    {
      _logger.LogWarning("User profile not found for user {UserId}. Skipping deletion.", userId);
      return;
    }

    var profile = profileResult.Value;

    // If the user has cancelled the deletion (logged in), skip
    if (!profile.IsDeleted)
    {
      _logger.LogInformation("User {UserId} has cancelled their account deletion. Skipping.", userId);
      return;
    }

    // Permanently delete the identity user (ASP.NET Identity)
    var deleteIdentityResult = await _identityService.DeleteUserAsync(userId);
    if (deleteIdentityResult.IsError)
    {
      _logger.LogError("Failed to delete identity for user {UserId}.", userId);
      throw new InvalidOperationException($"Failed to permanently delete identity for user {userId}.");
    }

    _logger.LogInformation("Permanently deleted account for user {UserId}.", userId);
  }
}
