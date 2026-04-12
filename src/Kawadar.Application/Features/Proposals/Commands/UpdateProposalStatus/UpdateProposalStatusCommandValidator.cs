using FluentValidation;
using Kawadar.Domain.Proposals.Enums;

namespace Kawadar.Application.Features.Proposals.Commands.UpdateProposalStatus;


public class UpdateProposalStatusCommandValidator : AbstractValidator<UpdateProposalStatusCommand>
{


  public UpdateProposalStatusCommandValidator()
  {
    RuleFor(x => x.ProposalId).NotEmpty().WithMessage("proposal id can not be empty");

    RuleFor(x => x.NewProposalStatus).Must(x => x == JobProposalStatus.Excluded).WithMessage("proposal status must be  excluded");
  }
}