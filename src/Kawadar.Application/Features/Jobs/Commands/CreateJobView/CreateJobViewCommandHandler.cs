using Kawadar.Application.Common.Errors;
using Kawadar.Application.Common.Interfaces;
using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.Jobs.JobViews;
using MediatR;

namespace Kawadar.Application.Features.Jobs.Commands.CreateJobView;

public class CreateJobViewCommandHandler : IRequestHandler<CreateJobViewCommand, Result<Created>>
{
  private readonly IUser _user;
  private readonly IJobsRepository _jobsRepository;
  private readonly IJobViewRepository _jobViewRepository;
  private readonly IUsersRepository _usersRepository;
  private readonly IUnitOfWork _unitOfWork;
  private readonly IRecommendationService _recommendationService;

  public CreateJobViewCommandHandler(
    IUser user,
    IJobsRepository jobsRepository,
    IJobViewRepository jobViewRepository,
    IUsersRepository usersRepository,
    IUnitOfWork unitOfWork
    , IRecommendationService recommendationService)
  {
    _user = user;
    _jobsRepository = jobsRepository;
    _jobViewRepository = jobViewRepository;
    _usersRepository = usersRepository;
    _unitOfWork = unitOfWork;
    _recommendationService = recommendationService;
  }

  public async Task<Result<Created>> Handle(CreateJobViewCommand request, CancellationToken cancellationToken)
  {
    var userId = _user.Id;
    if (userId is null) return ApplicationErrors.UserIsNotAuthenticated;

    var userProfileResult = await _usersRepository.GetUserProfileByUserIdAsync(userId);
    if (userProfileResult.IsError) return userProfileResult.Errors;
    var userProfile = userProfileResult.Value;

    var jobResult = await _jobsRepository.GetJobBySlugAsync(Uri.UnescapeDataString(request.Slug));
    if (jobResult.IsError) return jobResult.Errors;
    var job = jobResult.Value;

    // Check if the user has already viewed this job
    var alreadyViewed = await _jobViewRepository.HasViewedAsync(job.Id, userProfile.Id);
    if (alreadyViewed)
      return Result.Created;

    var jobViewResult = JobView.Create(job.Id, userProfile.Id);
    if (jobViewResult.IsError) return jobViewResult.Errors;

    await _jobViewRepository.AddAsync(jobViewResult.Value);
    await _unitOfWork.SaveChangesAsync(cancellationToken);

    var feedback = new RecommendationFeedback("star", userProfile.Id, job.Id.ToString());

    await _recommendationService.InsertFeedbackAsync(new[] { feedback }, cancellationToken);


    return Result.Created;
  }
}
