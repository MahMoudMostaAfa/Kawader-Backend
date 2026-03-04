using Kawadar.Application.Common.Errors;
using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.Jobs.JobQuestions;
using MediatR;

namespace Kawadar.Application.Features.Jobs.Commands.UpdateJobQuestions;

public class UpdateJobQuestionsCommandHandler : IRequestHandler<UpdateJobQuestionsCommand, Result<Updated>>
{
  private readonly IUser _user;
  private readonly IJobsRepository _jobsRepository;
  private readonly IUsersRepository _usersRepository;
  private readonly IUnitOfWork _unitOfWork;

  public UpdateJobQuestionsCommandHandler(
    IUser user,
    IJobsRepository jobsRepository,
    IUsersRepository usersRepository,
    IUnitOfWork unitOfWork)
  {
    _user = user;
    _jobsRepository = jobsRepository;
    _usersRepository = usersRepository;
    _unitOfWork = unitOfWork;
  }

  public async Task<Result<Updated>> Handle(UpdateJobQuestionsCommand request, CancellationToken cancellationToken)
  {
    var userId = _user.Id;
    if (userId is null) return ApplicationErrors.UserIsNotAuthenticated;

    var userProfileResult = await _usersRepository.GetUserProfileByUserIdAsync(userId);
    if (userProfileResult.IsError) return userProfileResult.Errors;
    var userProfile = userProfileResult.Value;

    var jobResult = await _jobsRepository.GetJobBySlugAsync(request.Slug);
    if (jobResult.IsError) return jobResult.Errors;
    var job = jobResult.Value;

    if (job.PostedById != userProfile.Id)
      return ApplicationErrors.UnauthorizedAccess;

    // Determine which existing questions to keep, update, or remove
    var incomingIds = request.Questions
      .Where(q => q.Id.HasValue)
      .Select(q => q.Id!.Value)
      .ToHashSet();

    // Remove questions not in the incoming list
    var existingQuestionIds = job.Questions.Select(q => q.Id).ToList();
    foreach (var questionId in existingQuestionIds)
    {
      if (!incomingIds.Contains(questionId))
      {
        var removeResult = job.RemoveQuestion(questionId);
        if (removeResult.IsError) return removeResult.Errors;
      }
    }

    // Update existing and add new questions
    int displayOrder = 1;
    foreach (var item in request.Questions)
    {
      if (item.Id.HasValue)
      {
        // Update existing question
        var existingQuestion = job.Questions.FirstOrDefault(q => q.Id == item.Id.Value);
        if (existingQuestion is not null)
        {
          existingQuestion.Update(item.Question, item.IsRequired, displayOrder);
        }
      }
      else
      {
        // Add new question
        var questionResult = JobQuestion.Create(item.Question, item.IsRequired);
        if (questionResult.IsError) return questionResult.Errors;

        var newQuestion = questionResult.Value;
        newQuestion.Update(item.Question, item.IsRequired, displayOrder);

        var addResult = job.AddQuestion(newQuestion);
        if (addResult.IsError) return addResult.Errors;
      }

      displayOrder++;
    }

    await _unitOfWork.SaveChangesAsync(cancellationToken);

    return Result.Updated;
  }
}
