using Kawadar.Application.Common.Constants;
using Kawadar.Application.Common.Errors;
using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Application.Common.Interfaces.Caching;
using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.Jobs;
using MediatR;

namespace Kawadar.Application.Features.Jobs.Commands.UpdateJobQuestion;

public class UpdateJobQuestionCommandHandler : IRequestHandler<UpdateJobQuestionCommand, Result<Updated>>
{
  private readonly IUser _user;
  private readonly IJobsRepository _jobsRepository;
  private readonly IUsersRepository _usersRepository;
  private readonly IUnitOfWork _unitOfWork;
  private readonly ICacheInvalidator _cacheInvalidator;

  public UpdateJobQuestionCommandHandler(
    IUser user,
    IJobsRepository jobsRepository,
    IUsersRepository usersRepository,
    IUnitOfWork unitOfWork,
    ICacheInvalidator cacheInvalidator)
  {
    _user = user;
    _jobsRepository = jobsRepository;
    _usersRepository = usersRepository;
    _unitOfWork = unitOfWork;
    _cacheInvalidator = cacheInvalidator;
  }

  public async Task<Result<Updated>> Handle(UpdateJobQuestionCommand request, CancellationToken cancellationToken)
  {
    var userId = _user.Id;
    if (userId is null) return ApplicationErrors.UserIsNotAuthenticated;

    var userProfileResult = await _usersRepository.GetUserProfileByUserIdAsync(userId);
    if (userProfileResult.IsError) return userProfileResult.Errors;
    var userProfile = userProfileResult.Value;

    var jobResult = await _jobsRepository.GetJobBySlugAsync(Uri.UnescapeDataString(request.Slug));
    if (jobResult.IsError) return jobResult.Errors;
    var job = jobResult.Value;

    if (job.PostedById != userProfile.Id)
      return ApplicationErrors.UnauthorizedAccess;

    var existingQuestion = job.Questions.FirstOrDefault(q => q.Id == request.QuestionId);
    if (existingQuestion is null)
      return JobErrors.JobQuestionNotFound;

    existingQuestion.Update(request.Question, request.IsRequired, existingQuestion.DisplayOrder);

    await _unitOfWork.SaveChangesAsync(cancellationToken);
    await _cacheInvalidator.EvictByTagAsync(CacheTags.JobsAll, cancellationToken);

    return Result.Updated;
  }
}
