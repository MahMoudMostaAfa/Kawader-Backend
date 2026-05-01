using FluentValidation;

namespace Kawadar.Application.Features.WalletAndPayments.Commands.DeactivatePayoutAccount;

public class DeactivatePayoutAccountCommandValidator : AbstractValidator<DeactivatePayoutAccountCommand>
{
  public DeactivatePayoutAccountCommandValidator()
  {
    RuleFor(c => c.AccountId)
      .NotEmpty().WithMessage("Account ID is required.");
  }
}
