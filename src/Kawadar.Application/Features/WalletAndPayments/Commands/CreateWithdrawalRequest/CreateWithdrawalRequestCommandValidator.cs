using FluentValidation;

namespace Kawadar.Application.Features.WalletAndPayments.Commands.CreateWithdrawalRequest;

public class CreateWithdrawalRequestCommandValidator : AbstractValidator<CreateWithdrawalRequestCommand>
{
  public CreateWithdrawalRequestCommandValidator()
  {
    RuleFor(c => c.Amount)
      .GreaterThan(0).WithMessage("Amount must be greater than zero.");

    RuleFor(c => c.PayoutAccountId)
      .NotEmpty().WithMessage("Payout account ID is required.");
  }
}
