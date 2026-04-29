using FluentValidation;

namespace Kawadar.Application.Features.Contracts.Commands.EditContractDeadline;


public class EditContractDeadlineCommandValidator : AbstractValidator<EditContractDeadlineCommand>
{

  public EditContractDeadlineCommandValidator()
  {
    RuleFor(x => x.ContractId)
        .NotEmpty().WithMessage("Contract ID is required.")
      ;

    RuleFor(x => x.NewDeadline)
      .GreaterThan(DateTime.UtcNow.AddDays(1)).WithMessage("New deadline must be in the future At last one day comming.");
  }
}