using FluentValidation;

namespace Kawadar.Application.Features.WalletAndPayments.Commands.UpdatePayoutAccount;

public class UpdatePayoutAccountCommandValidator : AbstractValidator<UpdatePayoutAccountCommand>
{
  public UpdatePayoutAccountCommandValidator()
  {
    RuleFor(c => c.AccountId)
      .NotEmpty().WithMessage("Account ID is required.");

    RuleFor(c => c.DisplayName)
      .NotEmpty().WithMessage("Display name is required.")
      .MaximumLength(100).WithMessage("Display name must not exceed 100 characters.");

    RuleFor(c => c.AccountDetails)
      .NotNull().WithMessage("Account details are required.");
  }
}
