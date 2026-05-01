using FluentValidation;

namespace Kawadar.Application.Features.WalletAndPayments.Commands.AddPayoutAccount;

public class AddPayoutAccountCommandValidator : AbstractValidator<AddPayoutAccountCommand>
{
  public AddPayoutAccountCommandValidator()
  {
    RuleFor(c => c.PayoutType)
      .IsInEnum().WithMessage("Invalid payout type.");

    RuleFor(c => c.DisplayName)
      .NotEmpty().WithMessage("Display name is required.")
      .MaximumLength(100).WithMessage("Display name must not exceed 100 characters.");

    RuleFor(c => c.AccountDetails)
      .NotNull().WithMessage("Account details are required.");
  }
}
