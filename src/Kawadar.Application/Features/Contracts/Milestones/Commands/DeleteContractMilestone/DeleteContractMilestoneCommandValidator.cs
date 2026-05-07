using FluentValidation;

namespace Kawadar.Application.Features.Contracts.Milestones.Commands.DeleteContractMilestone;

public class DeleteContractMilestoneCommandValidator : AbstractValidator<DeleteContractMilestoneCommand>
{
  public DeleteContractMilestoneCommandValidator()
  {
    RuleFor(x => x.ContractId).NotEmpty().WithMessage("ContractId is required");
    RuleFor(x => x.MilestoneId).NotEmpty().WithMessage("MilestoneId is required");
  }
}
