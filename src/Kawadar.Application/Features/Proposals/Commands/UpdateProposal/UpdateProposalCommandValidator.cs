using FluentValidation;
using Kawadar.Application.Features.Proposals.Dtos;

namespace Kawadar.Application.Features.Proposals.Commands.UpdateProposal;

public class UpdateProposalCommandValidator : AbstractValidator<UpdateProposalCommand>
{
  public UpdateProposalCommandValidator()
  {
    RuleFor(x => x.ProposalId).NotEmpty();
    When(x => x.CoverLetter != null, () =>
    {
      RuleFor(x => x.CoverLetter).NotEmpty().Length(50, 800).WithMessage("Cover letter must be between 50 and 800 characters");
    });

    When(x => x.QuestionAnswerUpdateDtos != null, () =>
    {
      RuleForEach(x => x.QuestionAnswerUpdateDtos).ChildRules(x =>
      {
        x.RuleFor(q => q.QuestionAnswerId).NotEmpty();
        x.RuleFor(q => q.QuestionAnswer).NotEmpty().Length(1, 5000).WithMessage("Question answer must be between 1 and 5000 characters");
      });
    });

    When(x => x.MilestoneUpdateDtos != null, () =>
    {
      RuleForEach(x => x.MilestoneUpdateDtos).ChildRules(x =>
      {
        x.RuleFor(m => m.MilestoneId).NotEmpty();
        x.RuleFor(m => m.Title).NotEmpty().Length(1, 100).WithMessage("Milestone title must be between 1 and 100 characters");
        x.RuleFor(m => m.Description).NotEmpty().Length(1, 5000).WithMessage("Milestone description must be between 1 and 5000 characters");
        x.RuleFor(m => m.Amount).GreaterThan(0).WithMessage("Milestone amount must be greater than 0");
        x.RuleFor(m => m.DueDate).GreaterThan(DateTime.UtcNow).WithMessage("Milestone due date must be in the future");
      });
    });

    When(x => x.Amount != null, () =>
    {
      RuleFor(x => x.Amount).GreaterThan(0).WithMessage("Amount must be greater than 0");
    });

    When(x => x.EstimatedDays != null, () =>
    {
      RuleFor(x => x.EstimatedDays).GreaterThan(0).WithMessage("Estimated days must be greater than 0");
    });

    When(x => x.HourlyRate != null, () =>
    {
      RuleFor(x => x.HourlyRate).GreaterThan(0).WithMessage("Hourly rate must be greater than 0");
    });

    When(x => x.EstimatedHours != null, () =>
    {
      RuleFor(x => x.EstimatedHours).GreaterThan(0).WithMessage("Estimated hours must be greater than 0");
    });
  }
}

