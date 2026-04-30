namespace Kawadar.Application.Features.Proposals.Commands.CreateProposal;

using Kawadar.Domain.Proposals.Enums;
using FluentValidation;




public class CreateProposalCommandValidator : AbstractValidator<CreateProposalCommand>
{

  public CreateProposalCommandValidator()
  {
    RuleFor(x => x.CoverLetter).NotEmpty().WithMessage("Cover letter is required")
      .MinimumLength(50).WithMessage("Cover letter must be at least 50 characters long")
      .MaximumLength(200).WithMessage("Cover letter must be less than 200 characters long");

    RuleFor(x => x.JobProposalType).IsInEnum().WithMessage("Invalid Proposal Type");


    When(x => x.JobProposalType == JobProposalType.OneTime, () =>
    {
      RuleFor(x => x.Amount).NotNull().WithMessage("Amount is required for one time proposals")
        .GreaterThan(0).WithMessage("Amount must be greater than 0");

      RuleFor(x => x.EstimatedDays).NotNull().WithMessage("Estimated days is required for one time proposals")
        .GreaterThan(0).WithMessage("Estimated days must be greater than 0");
    });

    When(x => x.JobProposalType == JobProposalType.Hourly, () =>
    {
      RuleFor(x => x.HourlyRate).NotNull().WithMessage("Hourly rate is required for hourly proposals")
        .GreaterThan(0).WithMessage("Hourly rate must be greater than 0");

      RuleFor(x => x.EstimatedHours).NotNull().WithMessage("Estimated hours is required for hourly proposals")
        .GreaterThan(0).WithMessage("Estimated hours must be greater than 0");
    });

    When(x => x.JobProposalType == JobProposalType.MilestoneBased, () =>
    {
      RuleFor(x => x.MilestoneDtos).NotNull().WithMessage("Milestones are required for milestone based proposals")
        .Must(milestones => milestones != null && milestones.Count > 0).WithMessage("At least one milestone is required for milestone based proposals");

      RuleForEach(x => x.MilestoneDtos).ChildRules(milestone =>
      {
        milestone.RuleFor(m => m.Title).NotEmpty().WithMessage("Milestone title is required");
        milestone.RuleFor(m => m.Description).NotEmpty().WithMessage("Milestone description is required");
        milestone.RuleFor(m => m.Amount).GreaterThan(0).WithMessage("Milestone amount must be greater than 0");
        milestone.RuleFor(m => m.DueDate).GreaterThan(DateTime.UtcNow).WithMessage("Milestone due date must be in the future");
      });
    });

    When(x => x.QuestionAnswerDtos != null && x.QuestionAnswerDtos.Count > 0, () =>
    {
      RuleForEach(x => x.QuestionAnswerDtos).ChildRules(qa =>
      {
        qa.RuleFor(q => q.QuestionId).NotEmpty().WithMessage("Question ID is required");
        qa.RuleFor(q => q.QuestionAnswer).NotEmpty().WithMessage("Answer is required");
      });
    });


  }
}