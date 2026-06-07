using FluentValidation;

namespace Kawadar.Application.Features.Contracts.Commands.AcceptContractCompletion;

public class AcceptContractCompletionCommandValidator : AbstractValidator<AcceptContractCompletionCommand>
{
  public AcceptContractCompletionCommandValidator()
  {
    RuleFor(x => x.ContractId).NotEmpty().WithMessage("Contract ID is required.");
  }
}