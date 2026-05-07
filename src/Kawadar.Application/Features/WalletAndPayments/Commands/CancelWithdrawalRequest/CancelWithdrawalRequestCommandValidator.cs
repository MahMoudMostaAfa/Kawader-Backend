using FluentValidation;

namespace Kawadar.Application.Features.WalletAndPayments.Commands.CancelWithdrawalRequest;

public class CancelWithdrawalRequestCommandValidator : AbstractValidator<CancelWithdrawalRequestCommand>
{
  public CancelWithdrawalRequestCommandValidator()
  {
    RuleFor(c => c.WithdrawalRequestId)
      .NotEmpty().WithMessage("Withdrawal request ID is required.");
  }
}
