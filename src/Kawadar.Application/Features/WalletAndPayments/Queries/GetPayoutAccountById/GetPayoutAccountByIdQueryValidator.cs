using FluentValidation;

namespace Kawadar.Application.Features.WalletAndPayments.Queries.GetPayoutAccountById;

public class GetPayoutAccountByIdQueryValidator : AbstractValidator<GetPayoutAccountByIdQuery>
{
  public GetPayoutAccountByIdQueryValidator()
  {
    RuleFor(q => q.AccountId)
      .NotEmpty().WithMessage("Account ID is required.");
  }
}
