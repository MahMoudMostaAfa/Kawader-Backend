using FluentValidation;

namespace Kawadar.Application.Features.Proposals.Commands.GenerateProposal;

public class GenerateProposalCommandValidator : AbstractValidator<GenerateProposalCommand>
{
  public GenerateProposalCommandValidator()
  {
    RuleFor(x => x.JobId).NotEmpty().WithMessage("JobId is required.");
  }
}