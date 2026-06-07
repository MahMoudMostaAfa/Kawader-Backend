using FluentValidation;

namespace Kawadar.Application.Features.WalletAndPayments.Commands.ApproveWithdrawalRequest;

public class ApproveWithdrawalRequestCommandValidator : AbstractValidator<ApproveWithdrawalRequestCommand>
{
  public ApproveWithdrawalRequestCommandValidator()
  {
    RuleFor(x => x.WithdrawalRequestId)
      .NotEmpty().WithMessage("Withdrawal request ID is required.");
  }
}
