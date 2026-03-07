using Kawadar.Application.Common.Errors;
using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Application.Features.Jobs.Commands.UpdateJob;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.Jobs;
using Kawadar.Domain.Jobs.Enums;
using MediatR;

public class UpdateJobCommandHandler : IRequestHandler<UpdateJobCommand, Result<Updated>>
{
  private readonly IJobsRepository _jobRepository;

  private readonly IUnitOfWork _unitOfWork;

  private readonly IIdentityService _identityService;

  private readonly IUser _user;

  private readonly IUsersRepository _usersRepository;
  public UpdateJobCommandHandler(IJobsRepository jobRepository, IUnitOfWork unitOfWork, IIdentityService identityService, IUser user, IUsersRepository usersRepository)
  {
    _jobRepository = jobRepository;
    _unitOfWork = unitOfWork;
    _identityService = identityService;
    _user = user;
    _usersRepository = usersRepository;


  }
  public async Task<Result<Updated>> Handle(UpdateJobCommand request, CancellationToken cancellationToken)
  {
    var userId = _user.Id;
    if (userId == null) return ApplicationErrors.UserIsNotAuthenticated;

    var userResult = await _identityService.GetUserByIdAsync(userId);
    if (userResult.IsError) return userResult.Errors;

    var userProfileResult = await _usersRepository.GetUserProfileByUserIdAsync(userId);
    if (userProfileResult.IsError) return userProfileResult.Errors;

    var userProfile = userProfileResult.Value;


    var JobResult = await _jobRepository.GetJobBySlugAsync(Uri.UnescapeDataString(request.Slug));
    if (JobResult.IsError) return JobResult.Errors;
    var job = JobResult.Value;
    if (job.PostedById != userProfile.Id) return ApplicationErrors.UnauthorizedAccess;

    if (job.JobStatus != JobStatus.Open) return JobErrors.JobNotOpen;

    var JobUpdateResult = job.Update(request.Title, request.Description, request.JobType, request.BudgetRange, request.HourlyRateRange, request.DurationInDays, request.ExperienceLevel, request.SpecilizationId);
    if (JobUpdateResult.IsError) return JobUpdateResult.Errors;

    await _unitOfWork.SaveChangesAsync(cancellationToken);

    return Result.Updated;
  }
}