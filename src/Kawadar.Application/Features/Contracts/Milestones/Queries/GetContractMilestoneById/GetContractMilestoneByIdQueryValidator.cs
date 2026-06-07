using FluentValidation;

namespace Kawadar.Application.Features.Contracts.Milestones.Queries.GetContractMilestoneById;

public class GetContractMilestoneByIdQueryValidator : AbstractValidator<GetContractMilestoneByIdQuery>
{
  public GetContractMilestoneByIdQueryValidator()
  {
    RuleFor(x => x.ContractId).NotEmpty().WithMessage("ContractId is required");
    RuleFor(x => x.MilestoneId).NotEmpty().WithMessage("MilestoneId is required");
  }
}
