using FluentValidation;

namespace Kawadar.Application.Features.Contracts.Milestones.Commands.UpdateContractMilestone;

public class UpdateContractMilestoneCommandValidator : AbstractValidator<UpdateContractMilestoneCommand>
{
  public UpdateContractMilestoneCommandValidator()
  {
    RuleFor(x => x.ContractId).NotEmpty().WithMessage("ContractId is required");
    RuleFor(x => x.MilestoneId).NotEmpty().WithMessage("MilestoneId is required");
    RuleFor(x => x.DueDate).NotEmpty().WithMessage("DueDate is required");
  }
}
