using Kawadar.Application.Common.Errors;
using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Domain.Common.Results;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Kawadar.Application.Features.Jobs.SavedJobs.Commands.RemoveSavedJob;

public class RemoveSavedJobCommandHandler : IRequestHandler<RemoveSavedJobCommand, Result<Deleted>>
{

  private readonly ILogger<RemoveSavedJobCommandHandler> _logger;
  private readonly ISavedJobsRepository _savedJobsRepository;
  private readonly IUnitOfWork _unitOfWork;

  private readonly IUser _user;
  private readonly IUsersRepository _usersRepository;


  public RemoveSavedJobCommandHandler(ILogger<RemoveSavedJobCommandHandler> logger, ISavedJobsRepository savedJobsRepository, IUnitOfWork unitOfWork, IUser user, IUsersRepository usersRepository)
  {
    _logger = logger;
    _savedJobsRepository = savedJobsRepository;
    _unitOfWork = unitOfWork;
    _user = user;
    _usersRepository = usersRepository;


  }




  public async Task<Result<Deleted>> Handle(RemoveSavedJobCommand request, CancellationToken cancellationToken)
  {

    var userId = _user.Id;
    if (userId is null) return ApplicationErrors.UserIsNotAuthenticated;


    var userProfileResult = await _usersRepository.GetUserProfileByUserIdAsync(userId);

    if (userProfileResult.IsError) return userProfileResult.Errors;
    var userProfile = userProfileResult.Value;
    var savedJobResult = await _savedJobsRepository.GetSavedJobByUserIdAndJobIdAsync(userProfile.Id, request.JobId);
    if (savedJobResult.IsError) return savedJobResult.Errors;
    var savedJob = savedJobResult.Value;

    await _savedJobsRepository.RemoveSavedJobAsync(savedJob);
    await _unitOfWork.SaveChangesAsync(cancellationToken);
    _logger.LogInformation("User {userId} removed saved job {jobId} successfully.", userId, request.JobId);


    return Result.Deleted;
  }
}