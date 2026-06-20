using Kawadar.Application.Common.Constants;
using Kawadar.Application.Common.Errors;
using Kawadar.Application.Common.Interfaces;
using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Application.Common.Interfaces.Caching;
using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Domain.Common.Constants;
using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.Jobs.Commands.DeleteJob;

public class DeleteJobCommandHandler : IRequestHandler<DeleteJobCommand, Result<Deleted>>
{
  private readonly IUser _user;
  private readonly IJobsRepository _jobsRepository;
  private readonly IUsersRepository _usersRepository;
  private readonly IStorageClient _storageClient;
  private readonly IUnitOfWork _unitOfWork;
  private readonly ICacheInvalidator _cacheInvalidator;
  private readonly IIdentityService _identityService;

  public DeleteJobCommandHandler(
    IUser user,
    IJobsRepository jobsRepository,
    IUsersRepository usersRepository,
    IStorageClient storageClient,
    IUnitOfWork unitOfWork,
    ICacheInvalidator cacheInvalidator,
    IIdentityService identityService)
  {
    _user = user;
    _jobsRepository = jobsRepository;
    _usersRepository = usersRepository;
    _storageClient = storageClient;
    _unitOfWork = unitOfWork;
    _cacheInvalidator = cacheInvalidator;
    _identityService = identityService;
  }

  public async Task<Result<Deleted>> Handle(DeleteJobCommand request, CancellationToken cancellationToken)
  {
    var userId = _user.Id;
    if (userId is null) return ApplicationErrors.UserIsNotAuthenticated;

    var userProfileResult = await _usersRepository.GetUserProfileByUserIdAsync(userId);
    if (userProfileResult.IsError) return userProfileResult.Errors;
    var userProfile = userProfileResult.Value;

    var jobResult = await _jobsRepository.GetJobBySlugAsync(Uri.UnescapeDataString(request.Slug));
    if (jobResult.IsError) return jobResult.Errors;
    var job = jobResult.Value;

    if (userProfile.ProfileType != Domain.UserProfiles.Enums.ProfileType.Admin && job.PostedById != userProfile.Id)
      return ApplicationErrors.UnauthorizedAccess;

    if (userProfile.ProfileType == Domain.UserProfiles.Enums.ProfileType.Admin)
    {
        var permissions = await _identityService.GetUserClaimsAsync(userProfile.UserId);
        if (!permissions.Value.Contains(("Permission", Permissions.ViewViolations)) && !permissions.Value.Contains(("Permission", Permissions.ViewJobReports)))
            return ApplicationErrors.UnauthorizedAccess;
    }

    // Delete uploaded attachments from blob storage
    foreach (var attachment in job.Attachments)
    {
      if (attachment.File.MimeType != "link")
      {
        await _storageClient.DeleteFileAsync(attachment.File.FileUrl, Containers.JobAttachements);
      }
    }

    _jobsRepository.Delete(job);

    await _unitOfWork.SaveChangesAsync(cancellationToken);

    await _cacheInvalidator.EvictByTagAsync(CacheTags.JobsAll, cancellationToken);

    return Result.Deleted;
  }
}
