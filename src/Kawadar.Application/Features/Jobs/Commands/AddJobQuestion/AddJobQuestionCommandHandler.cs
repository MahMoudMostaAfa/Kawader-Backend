using Kawadar.Application.Common.Constants;
using Kawadar.Application.Common.Errors;
using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Application.Common.Interfaces.Caching;
using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.Jobs.JobQuestions;
using MediatR;

namespace Kawadar.Application.Features.Jobs.Commands.AddJobQuestion;

public class AddJobQuestionCommandHandler : IRequestHandler<AddJobQuestionCommand, Result<Created>>
{
  private readonly IUser _user;
  private readonly IJobsRepository _jobsRepository;
  private readonly IUsersRepository _usersRepository;
  private readonly IUnitOfWork _unitOfWork;
  private readonly ICacheInvalidator _cacheInvalidator;

  public AddJobQuestionCommandHandler(
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

  public async Task<Result<Created>> Handle(AddJobQuestionCommand request, CancellationToken cancellationToken)
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

    var nextDisplayOrder = job.Questions.Count > 0
      ? job.Questions.Max(q => q.DisplayOrder) + 1
      : 1;

    var questionResult = JobQuestion.Create(request.Question, request.IsRequired, nextDisplayOrder);
    if (questionResult.IsError) return questionResult.Errors;

    var addResult = job.AddQuestion(questionResult.Value);
    if (addResult.IsError) return addResult.Errors;

    await _unitOfWork.SaveChangesAsync(cancellationToken);
    await _cacheInvalidator.EvictByTagAsync(CacheTags.JobsAll, cancellationToken);
    return Result.Created;
  }
}
