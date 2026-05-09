using Kawadar.Application.Common.Errors;
using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.Proposals;
using Kawadar.Domain.Proposals.ProposalMilestones;
using Kawadar.Domain.Proposals.QuestionAnswers;
using MediatR;

namespace Kawadar.Application.Features.Proposals.Commands.CreateProposal;


public class CreateProposalCommandHandler : IRequestHandler<CreateProposalCommand, Result<Created>>
{

  private readonly IUser _user;
  private readonly IJobsRepository _jobsRepository;
  private readonly IUsersRepository _usersRepository;
  private readonly IProposalsRepository _proposalsRepository;
  private readonly IUnitOfWork _unitOfWork;
  public CreateProposalCommandHandler(IUser user, IJobsRepository jobsRepository, IUsersRepository usersRepository, IProposalsRepository proposalsRepository, IUnitOfWork unitOfWork)
  {
    _user = user;
    _jobsRepository = jobsRepository;
    _usersRepository = usersRepository;
    _proposalsRepository = proposalsRepository;
    _unitOfWork = unitOfWork;
  }
  public async Task<Result<Created>> Handle(CreateProposalCommand request, CancellationToken cancellationToken)
  {
    var userId = _user.Id;
    if (userId is null) return ApplicationErrors.UserIsNotAuthenticated;

    var UserProfileResult = await _usersRepository.GetUserProfileByUserIdAsync(userId);
    if (UserProfileResult.IsError) return UserProfileResult.Errors;
    var userProfile = UserProfileResult.Value;

    var jobResult = await _jobsRepository.GetJobsAsync(request.JobId);

    if (jobResult.IsError) return jobResult.Errors;

    var job = jobResult.Value;

    if (job.PostedById == userProfile.Id)
      return Error.Forbidden(description: "You cannot submit a proposal to your own job.");

    if (job.JobType == Domain.Jobs.Enums.JobType.FixedPrice && (request.JobProposalType == Domain.Proposals.Enums.JobProposalType.Hourly))

      return Error.Validation(description: "Hourly proposals are not allowed for fixed price jobs.");

    if (job.JobType == Domain.Jobs.Enums.JobType.Hourly && (request.JobProposalType == Domain.Proposals.Enums.JobProposalType.MilestoneBased || request.JobProposalType == Domain.Proposals.Enums.JobProposalType.OneTime))

      return Error.Validation(description: "Fixed price proposals are not allowed for hourly jobs.");

    var existsResult = await _proposalsRepository.ProposalExistsForJobAndFreelancerAsync(request.JobId, userProfile.Id, cancellationToken);
    if (existsResult.IsError) return existsResult.Errors;
    if (existsResult.Value) return JobProposalErrors.ProposalAlreadyExistsForJob;

    var jobQuestions = job.Questions.ToHashSet();
    var questionIds = jobQuestions.Select(q => q.Id).ToHashSet();
    // check if the provided question answers are valid
    foreach (var questionAnswer in request.QuestionAnswerDtos ?? [])
    {
      if (!questionIds.Contains(questionAnswer.QuestionId))
      {
        return Error.Validation("Invalid question answer provided.");
      }
    }
    // check that all required questions are answered
    foreach (var question in jobQuestions.Where(q => q.IsRequired))
    {
      if (!request.QuestionAnswerDtos!.Any(qa => qa.QuestionId == question.Id))
      {
        return Error.Validation("All required questions must be answered.");
      }
    }

    var proposalResult = JobProposal.Create(request.JobId, userProfile.Id, request.CoverLetter, request.JobProposalType, request.Amount, request.HourlyRate, request.EstimatedHours, request.EstimatedDays);
    if (proposalResult.IsError) return proposalResult.Errors;

    var proposal = proposalResult.Value;

    if (request.JobProposalType == Domain.Proposals.Enums.JobProposalType.MilestoneBased)
    {
      foreach (var milestonDto in request.MilestoneDtos ?? [])
      {
        var milestone = ProposalMilestone.Create(proposal.Id, milestonDto.Title, milestonDto.Description, milestonDto.Amount, milestonDto.DueDate);
        if (milestone.IsError) return milestone.Errors;
        proposal.AddMilestone(milestone.Value);
      }
    }


    foreach (var questionAnswer in request.QuestionAnswerDtos ?? [])
    {

      var QuestionAnswerResult = ProposalQuestionAnswer.Create(proposal.Id, questionAnswer.QuestionId, questionAnswer.QuestionAnswer);
      if (QuestionAnswerResult.IsError) return QuestionAnswerResult.Errors;
      proposal.AddQuestionAnswer(QuestionAnswerResult.Value);
    }


    await _proposalsRepository.AddAsync(proposal, cancellationToken);


    await _unitOfWork.SaveChangesAsync(cancellationToken);

    return Result.Created;


  }
}