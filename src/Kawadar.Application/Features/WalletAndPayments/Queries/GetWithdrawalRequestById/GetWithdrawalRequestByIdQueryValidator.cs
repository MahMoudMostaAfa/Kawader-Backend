using FluentValidation;

namespace Kawadar.Application.Features.WalletAndPayments.Queries.GetWithdrawalRequestById;

public class GetWithdrawalRequestByIdQueryValidator : AbstractValidator<GetWithdrawalRequestByIdQuery>
{
  public GetWithdrawalRequestByIdQueryValidator()
  {
    RuleFor(q => q.WithdrawalRequestId)
      .NotEmpty().WithMessage("Withdrawal request ID is required.");
  }
}
