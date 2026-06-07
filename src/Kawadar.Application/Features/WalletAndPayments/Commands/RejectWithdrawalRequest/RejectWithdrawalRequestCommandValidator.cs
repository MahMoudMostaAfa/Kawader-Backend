using FluentValidation;

namespace Kawadar.Application.Features.WalletAndPayments.Commands.RejectWithdrawalRequest;

public class RejectWithdrawalRequestCommandValidator : AbstractValidator<RejectWithdrawalRequestCommand>
{
  public RejectWithdrawalRequestCommandValidator()
  {
    RuleFor(x => x.WithdrawalRequestId)
      .NotEmpty().WithMessage("Withdrawal request ID is required.");

    RuleFor(x => x.Reason)
      .NotEmpty().WithMessage("Rejection reason is required.")
      .MaximumLength(500).WithMessage("Rejection reason must be 500 characters or fewer.");
  }
}
