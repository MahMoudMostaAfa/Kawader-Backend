using FluentValidation;

namespace Kawadar.Application.Features.Contracts.Commands.CancelContract;

public class CancelContractCommandValidator : AbstractValidator<CancelContractCommand>
{
  public CancelContractCommandValidator()
  {
    RuleFor(x => x.ContractId)
      .NotEmpty().WithMessage("Contract ID is required.");

  }
}