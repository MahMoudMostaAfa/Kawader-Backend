using FluentValidation;

namespace Kawadar.Application.Features.WalletAndPayments.Queries.GetAdminWithdrawals;

public class GetAdminWithdrawalsQueryValidator : AbstractValidator<GetAdminWithdrawalsQuery>
{
  public GetAdminWithdrawalsQueryValidator()
  {
    RuleFor(x => x.Page).GreaterThanOrEqualTo(1);
    RuleFor(x => x.PageSize).GreaterThanOrEqualTo(1).LessThanOrEqualTo(50);
  }
}
