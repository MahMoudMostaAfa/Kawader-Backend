using FluentValidation;

namespace Kawadar.Application.Features.WalletAndPayments.Commands.SetDefaultPayoutAccount;

public class SetDefaultPayoutAccountCommandValidator : AbstractValidator<SetDefaultPayoutAccountCommand>
{
  public SetDefaultPayoutAccountCommandValidator()
  {
    RuleFor(c => c.AccountId)
      .NotEmpty().WithMessage("Account ID is required.");
  }
}
