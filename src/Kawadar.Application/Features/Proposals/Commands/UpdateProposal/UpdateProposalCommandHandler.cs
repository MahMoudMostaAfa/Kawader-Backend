using Kawadar.Application.Common.Errors;
using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.Proposals.Enums;
using Kawadar.Domain.Proposals.ProposalMilestones;
using MediatR;

namespace Kawadar.Application.Features.Proposals.Commands.UpdateProposal;

public class UpdateProposalCommandHandler : IRequestHandler<UpdateProposalCommand, Result<Updated>>
{

  private readonly IUser _user;

  private readonly IUsersRepository _usersRepository;
  private readonly IProposalsRepository _proposalsRepository;
  private readonly IUnitOfWork _unitOfWork;

  public UpdateProposalCommandHandler(IUser user, IUsersRepository usersRepository, IProposalsRepository proposalsRepository, IUnitOfWork unitOfWork)
  {
    _user = user;
    _usersRepository = usersRepository;
    _proposalsRepository = proposalsRepository;
    _unitOfWork = unitOfWork;
  }
  public async Task<Result<Updated>> Handle(UpdateProposalCommand request, CancellationToken cancellationToken)
  {
    var userId = _user.Id;
    if (userId == null) return ApplicationErrors.UserIsNotAuthenticated;

    var profileResult = await _usersRepository.GetUserProfileByUserIdAsync(userId);
    if (profileResult.IsError) return profileResult.Errors;
    var profile = profileResult.Value;

    var proposaLResult = await _proposalsRepository.GetByIdAsync(request.ProposalId, cancellationToken);
    if (proposaLResult.IsError) return proposaLResult.Errors;
    var proposal = proposaLResult.Value;

    if (proposal.ProposalType == JobProposalType.Hourly && (request.Amount != null || request.EstimatedDays != null)) return Error.Validation(description: "You cannot update amount or estimated days for hourly proposals.");

    if (proposal.ProposalType == JobProposalType.OneTime && (request.HourlyRate != null || request.EstimatedHours != null)) return Error.Validation(description: "You cannot update hourly rate or estimated hours for fixed price proposals.");

    if ((proposal.ProposalType == JobProposalType.OneTime || proposal.ProposalType == JobProposalType.Hourly) && request.MilestoneUpdateDtos != null) return Error.Validation("proposals.milestones", description: "this action valid only for milestone based proposal  ");



    if (proposal.FreelancerId != profile.Id) return Error.Forbidden(description: "You can only update your own proposals.");

    var proposalUpdateResult = proposal.Update(request.CoverLetter, null, request.Amount, request.HourlyRate, request.EstimatedHours, request.EstimatedDays);

    if (proposalUpdateResult.IsError) return proposalUpdateResult.Errors;


    // update question answers
    if (request.QuestionAnswerUpdateDtos != null)
    {

      foreach (var questionAnswer in request.QuestionAnswerUpdateDtos)
      {
        var QuestionAnswerResult = proposal.QuestionAnswers.FirstOrDefault(qa => qa.Id == questionAnswer.QuestionAnswerId);
        if (QuestionAnswerResult == null) return Error.NotFound(description: $"Question answer with id {questionAnswer.QuestionAnswerId} not found in this proposal.");
        var updateQuestionAnswerResult = QuestionAnswerResult.Update(questionAnswer.QuestionAnswer);
        if (updateQuestionAnswerResult.IsError) return updateQuestionAnswerResult.Errors;

      }
    }

    // delete milestones that are not in the request
    if (request.MilestoneUpdateDtos != null && proposal.ProposalType == JobProposalType.MilestoneBased)
    {
      var milestonesToDelete = proposal.Milestones.Where(m => !request.MilestoneUpdateDtos.Any(dto => dto.MilestoneId == m.Id)).ToList();
      foreach (var milestone in milestonesToDelete)
      {
        var deleteMilestoneResult = proposal.RemoveMilestone(milestone);
        if (deleteMilestoneResult.IsError) return deleteMilestoneResult.Errors;
      }
    }

    // update milestones 
    if (request.MilestoneUpdateDtos != null && proposal.ProposalType == JobProposalType.MilestoneBased)
    {
      foreach (var milestone in request.MilestoneUpdateDtos)
      {
        var milestoneResult = proposal.Milestones.FirstOrDefault(m => m.Id == milestone.MilestoneId);
        // if milestoneResult is null, it means that this is a new milestone that needs to be added, otherwise, it needs to be updated
        if (milestoneResult == null)
        {

          var maxOrder = proposal.Milestones.Count() == 0 ? 0 : proposal.Milestones.Max(m => m.DisplayOrder);
          // add new milestone
          var milestoneCreateResult = ProposalMilestone.Create(proposal.Id, milestone.Title, milestone.Description, milestone.Amount, milestone.DueDate, maxOrder + 1);
          if (milestoneCreateResult.IsError) return milestoneCreateResult.Errors;

          var addMilestoneResult = proposal.AddMilestone(milestoneCreateResult.Value);
          if (addMilestoneResult.IsError) return addMilestoneResult.Errors;
          continue;
        }

        var updateMilestoneResult = milestoneResult.Update(milestone.Title, milestone.Description, milestone.Amount, milestone.DueDate, null, null);
        if (updateMilestoneResult.IsError) return updateMilestoneResult.Errors;
      }
    }



    await _unitOfWork.SaveChangesAsync(cancellationToken);
    return Result.Updated;

  }
}