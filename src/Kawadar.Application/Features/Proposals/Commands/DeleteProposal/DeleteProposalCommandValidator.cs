using FluentValidation;

namespace Kawadar.Application.Features.Proposals.Commands.DeleteProposal;

public class DeleteProposalCommandValidator : AbstractValidator<DeleteProposalCommand>
{
  public DeleteProposalCommandValidator()
  {
    RuleFor(x => x.ProposalId).NotEmpty().WithMessage("proposal id can not be empty");
  }
}