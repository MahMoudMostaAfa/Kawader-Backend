using FluentValidation;

namespace Kawadar.Application.Features.Contracts.Commands.RejectContractCompletion;

public class RejectContractCompletionCommandValidator : AbstractValidator<RejectContractCompletionCommand>
{
  public RejectContractCompletionCommandValidator()
  {
    RuleFor(x => x.ContractId).NotEmpty().WithMessage("Contract ID is required.");
    RuleFor(x => x.Reason).NotEmpty().WithMessage("Rejection reason is required.");
  }
}