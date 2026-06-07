using FluentValidation;

namespace Kawadar.Application.Features.Contracts.Milestones.Queries.GetContractMilestones;

public class GetContractMilestonesQueryValidator : AbstractValidator<GetContractMilestonesQuery>
{
  public GetContractMilestonesQueryValidator()
  {
    RuleFor(x => x.ContractId).NotEmpty().WithMessage("ContractId is required");
  }
}
