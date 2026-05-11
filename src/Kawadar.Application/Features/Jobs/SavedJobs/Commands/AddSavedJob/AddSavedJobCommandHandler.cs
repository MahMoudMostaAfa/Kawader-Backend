using Kawadar.Application.Common.Errors;
using Kawadar.Application.Common.Interfaces;
using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.Jobs.Enums;
using Kawadar.Domain.Jobs.SavedJobs;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Kawadar.Application.Features.Jobs.SavedJobs.Commands.AddSavedJob;

// <summary>
// Command handler for adding a job to the user's saved jobs list.
// </summary>

public class AddSavedJobCommandHandler : IRequestHandler<AddSavedJobCommand, Result<Created>>
{

  private readonly ILogger<AddSavedJobCommandHandler> _logger;
  private readonly ISavedJobsRepository _savedJobsRepository;
  private readonly IUsersRepository _usersRepository;
  private readonly IUnitOfWork _unitOfWork;
  private readonly IUser _user;
  private readonly IJobsRepository _jobsRepository;
  private readonly IRecommendationService _recommendationService;

  public AddSavedJobCommandHandler(ILogger<AddSavedJobCommandHandler> logger, ISavedJobsRepository savedJobsRepository, IUnitOfWork unitOfWork, IUser user, IUsersRepository usersRepository, IJobsRepository jobsRepository, IRecommendationService recommendationService)
  {
    _logger = logger;
    _savedJobsRepository = savedJobsRepository;
    _unitOfWork = unitOfWork;
    _user = user;
    _usersRepository = usersRepository;
    _jobsRepository = jobsRepository;
    _recommendationService = recommendationService;

  }
  public async Task<Result<Created>> Handle(AddSavedJobCommand request, CancellationToken cancellationToken)
  {
    var userId = _user.Id;
    if (userId is null) return ApplicationErrors.UserIsNotAuthenticated;


    var userProfileResult = await _usersRepository.GetUserProfileByUserIdAsync(userId);
    if (userProfileResult.IsError) return userProfileResult.Errors;

    var userProfile = userProfileResult.Value;
    var jobResult = await _jobsRepository.GetJobByIdAsync(request.JobId);
    if (jobResult.IsError) return jobResult.Errors;
    var job = jobResult.Value;

    if (job.JobStatus != JobStatus.Open) return Error.NotFound("Jobs.NotFound", "The job you are trying to save does not exist or is not open.");

    if (job.PostedById == userProfile.Id) return Error.Conflict("Jobs.CannotSaveOwnJob", "You cannot save your own job.");

    var saveJobResult = SavedJob.Create(job.Id, userProfile.Id);
    if (saveJobResult.IsError) return saveJobResult.Errors;
    var savedJob = saveJobResult.Value;

    var savedJobResult = await _savedJobsRepository.AddSavedJobAsync(savedJob);
    if (savedJobResult.IsError) return savedJobResult.Errors;

    await _unitOfWork.SaveChangesAsync(cancellationToken);

    _logger.LogInformation("User {userId} saved job {jobId} successfully.", userId, job.Id);

    // Insert "star" feedback into Gorse — saving a job is a strong positive signal
    var feedback = new RecommendationFeedback("star", userProfile.Id, job.Id.ToString());
    await _recommendationService.InsertFeedbackAsync(new[] { feedback }, cancellationToken);

    return Result.Created;



  }
}