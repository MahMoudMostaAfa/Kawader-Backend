using FluentValidation;

namespace Kawadar.Application.Features.Proposals.Queries.GetProposalById;


public class GetProposalByIdQueryValidator : AbstractValidator<GetProposalByIdQuery>
{
  public GetProposalByIdQueryValidator()
  {
    RuleFor(x => x.ProposalId).NotEmpty().WithMessage("Proposal Id Can not be empty ");
  }
}