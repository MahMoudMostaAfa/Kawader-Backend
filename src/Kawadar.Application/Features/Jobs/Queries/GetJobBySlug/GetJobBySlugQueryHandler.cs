using AutoMapper;
using Kawadar.Application.Common.Errors;
using Kawadar.Application.Common.Interfaces;
using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Application.Features.Jobs.DTOs;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.Jobs.JobViews;
using MediatR;

namespace Kawadar.Application.Features.Jobs.Queries.GetJobBySlug;

public class GetJobBySlugQueryHandler : IRequestHandler<GetJobBySlugQuery, Result<JobDetailsDto>>
{
  private readonly IJobsRepository _jobsRepository;
  private readonly IJobViewRepository _jobViewRepository;
  private readonly IMapper _mapper;
  private readonly IIdentityService _identityService;
  private readonly IUser _user;
  private readonly IUsersRepository _usersRepository;
  private readonly IUnitOfWork _unitOfWork;
  private readonly IRecommendationService _recommendationService;

  public GetJobBySlugQueryHandler(
    IJobsRepository jobsRepository,
    IJobViewRepository jobViewRepository,
    IMapper mapper,
    IIdentityService identityService,
    IUser user,
    IUsersRepository usersRepository,
    IUnitOfWork unitOfWork,
    IRecommendationService recommendationService)
  {
    _jobsRepository = jobsRepository;
    _jobViewRepository = jobViewRepository;
    _mapper = mapper;
    _identityService = identityService;
    _user = user;
    _usersRepository = usersRepository;
    _unitOfWork = unitOfWork;
    _recommendationService = recommendationService;
  }
  public async Task<Result<JobDetailsDto>> Handle(GetJobBySlugQuery request, CancellationToken cancellationToken)
  {
    var userId = _user.Id;
    if (userId is null) return ApplicationErrors.UserIsNotAuthenticated;

    var jobResult = await _jobsRepository.GetJobBySlugAsync(Uri.UnescapeDataString(request.Slug));
    if (jobResult.IsError) return jobResult.Errors;

    var job = jobResult.Value;

    var UserProfileResult = await _usersRepository.GetUserProfileByIdAsync(job.PostedById);
    if (UserProfileResult.IsError) return UserProfileResult.Errors;
    var userProfile = UserProfileResult.Value;

    var posterIdentityResult = await _identityService.GetUserByIdAsync(userProfile.UserId);
    if (posterIdentityResult.IsError) return posterIdentityResult.Errors;
    var posterIdentity = posterIdentityResult.Value;

    // Record the view implicitly
    var viewerProfileResult = await _usersRepository.GetUserProfileByUserIdAsync(userId);
    if (!viewerProfileResult.IsError)
    {
      var viewerProfile = viewerProfileResult.Value;
      var alreadyViewed = await _jobViewRepository.HasViewedAsync(job.Id, viewerProfile.Id);
      if (!alreadyViewed)
      {
        var jobViewResult = JobView.Create(job.Id, viewerProfile.Id);
        if (!jobViewResult.IsError)
        {
          await _jobViewRepository.AddAsync(jobViewResult.Value);
          await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        var viewFeedback = new RecommendationFeedback("view", viewerProfile.Id, job.Id.ToString());
        var likeFeedback = new RecommendationFeedback("like", viewerProfile.Id, job.Id.ToString());
        await _recommendationService.InsertFeedbackAsync(new[] { viewFeedback, likeFeedback }, cancellationToken);
      }
    }

    var jobDetailsDto = _mapper.Map<JobDetailsDto>((job, posterIdentity, userProfile));

    return jobDetailsDto;

  }
}